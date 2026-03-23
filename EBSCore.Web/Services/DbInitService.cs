using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;

namespace EBSCore.Web.Services
{
    public class DbInitService : IHostedService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DbInitService> _logger;
        private readonly IHostEnvironment _env;

        public DbInitService(IConfiguration config, ILogger<DbInitService> logger, IHostEnvironment env)
        {
            _config = config;
            _logger = logger;
            _env = env;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var cs = _config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(cs))
            {
                _logger.LogWarning("DefaultConnection is empty; SQLite initialization skipped.");
                return;
            }

            var builder = new SqliteConnectionStringBuilder(cs);
            if (!Path.IsPathRooted(builder.DataSource))
            {
                builder.DataSource = Path.Combine(_env.ContentRootPath, builder.DataSource);
            }

            var directory = Path.GetDirectoryName(builder.DataSource);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var conn = new SqliteConnection(builder.ToString());
            await conn.OpenAsync(cancellationToken);

            await ApplyBootstrapScriptAsync(conn, cancellationToken);

            var applyLegacySchema = _config.GetValue<bool>("DbInit:ApplyLegacySqlScripts", false);
            if (applyLegacySchema)
            {
                // Optional compatibility mode for historical SQL Server script conversion.
                await ApplySchemaTablesAsync(conn, cancellationToken);
                await ApplyAdditionalSqlTableScriptsAsync(conn, cancellationToken);
                await CreateCoreCompatibilityTablesAsync(conn, cancellationToken);
                await SeedUsersAsync(conn, cancellationToken);
                await SeedMenuAsync(conn, cancellationToken);
            }

            _logger.LogInformation("SQLite database initialized at {DataSource}", builder.DataSource);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


        private async Task ApplyBootstrapScriptAsync(SqliteConnection conn, CancellationToken cancellationToken)
        {
            var scriptPath = Path.Combine(_env.ContentRootPath, "AppData", "bootstrap.sqlite.sql");
            if (!File.Exists(scriptPath))
            {
                _logger.LogWarning("SQLite bootstrap script not found at {Path}; skipping script bootstrap.", scriptPath);
                return;
            }

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
            await ExecuteAsync(conn, script, cancellationToken);
            _logger.LogInformation("SQLite bootstrap script applied from {Path}", scriptPath);
        }

        private async Task ApplySchemaTablesAsync(SqliteConnection conn, CancellationToken cancellationToken)
        {
            var schemaPath = Path.Combine(_env.ContentRootPath, "AppData", "SCHEMA22112025.sql");
            if (!File.Exists(schemaPath))
            {
                _logger.LogWarning("Schema history file not found at {Path}; skipping full table bootstrap.", schemaPath);
                return;
            }

            string content;
            try
            {
                content = await File.ReadAllTextAsync(schemaPath, Encoding.Unicode, cancellationToken);
            }
            catch
            {
                content = await File.ReadAllTextAsync(schemaPath, cancellationToken);
            }

            // Some historical scripts are UTF-16 with null characters.
            content = content.Replace("\0", string.Empty);

            var createStatements = ConvertSqlServerTableScriptToSqlite(content);
            var created = 0;
            foreach (var statement in createStatements)
            {
                try
                {
                    await ExecuteAsync(conn, statement, cancellationToken);
                    created++;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Skipped table DDL during SQLite conversion: {Sql}", statement);
                }
            }

            _logger.LogInformation("SQLite schema bootstrap applied {Count} CREATE TABLE statements from history schema.", created);
        }


        private async Task ApplyAdditionalSqlTableScriptsAsync(SqliteConnection conn, CancellationToken cancellationToken)
        {
            var appDataPath = Path.Combine(_env.ContentRootPath, "AppData");
            if (!Directory.Exists(appDataPath))
            {
                return;
            }

            var sqlFiles = Directory.GetFiles(appDataPath, "*.sql", SearchOption.TopDirectoryOnly)
                .Where(f => !f.EndsWith("SCHEMA22112025.sql", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var applied = 0;
            foreach (var file in sqlFiles)
            {
                string content;
                try
                {
                    content = await File.ReadAllTextAsync(file, Encoding.Unicode, cancellationToken);
                }
                catch
                {
                    content = await File.ReadAllTextAsync(file, cancellationToken);
                }

                content = content.Replace("\0", string.Empty);
                var createStatements = ConvertSqlServerTableScriptToSqlite(content);
                foreach (var statement in createStatements)
                {
                    try
                    {
                        await ExecuteAsync(conn, statement, cancellationToken);
                        applied++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Skipped SQL file table DDL conversion for {File}", Path.GetFileName(file));
                    }
                }
            }

            _logger.LogInformation("SQLite additional SQL bootstrap applied {Count} CREATE TABLE statements from AppData/*.sql scripts.", applied);
        }

        private static IEnumerable<string> ConvertSqlServerTableScriptToSqlite(string script)
        {
            var statements = new List<string>();
            var marker = "CREATE TABLE";
            var index = 0;

            while (index < script.Length)
            {
                var start = script.IndexOf(marker, index, StringComparison.OrdinalIgnoreCase);
                if (start < 0) break;

                var openParen = script.IndexOf('(', start);
                if (openParen < 0) break;

                var tableHeader = script[start..openParen].Trim();
                var tableName = NormalizeTableName(tableHeader);
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    index = openParen + 1;
                    continue;
                }

                var depth = 0;
                var closeParen = -1;
                for (var i = openParen; i < script.Length; i++)
                {
                    if (script[i] == '(') depth++;
                    else if (script[i] == ')')
                    {
                        depth--;
                        if (depth == 0)
                        {
                            closeParen = i;
                            break;
                        }
                    }
                }

                if (closeParen < 0) break;

                var body = script.Substring(openParen + 1, closeParen - openParen - 1);
                var ddl = BuildSqliteCreateTable(tableName, body);
                if (!string.IsNullOrWhiteSpace(ddl))
                {
                    statements.Add(ddl);
                }

                index = closeParen + 1;
            }

            return statements;
        }

        private static string NormalizeTableName(string createHeader)
        {
            // Expected: CREATE TABLE [schema].[table]
            var match = Regex.Match(createHeader, @"CREATE\s+TABLE\s+\[(?<schema>[^\]]+)\]\.\[(?<table>[^\]]+)\]", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return string.Empty;
            }

            var schema = match.Groups["schema"].Value;
            var table = match.Groups["table"].Value;
            var normalized = string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase)
                ? table
                : $"{schema}_{table}";

            return $"[{normalized}]";
        }

        private static string BuildSqliteCreateTable(string normalizedTableName, string body)
        {
            var lines = body
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            var columns = new List<string>();
            var primaryKeys = new List<string>();
            string? identityPkColumn = null;

            foreach (var raw in lines)
            {
                var line = raw.Trim().TrimEnd(',');

                if (line.StartsWith("CONSTRAINT", StringComparison.OrdinalIgnoreCase))
                {
                    if (line.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (Match col in Regex.Matches(line, "\\[([^\\]]+)\\]"))
                        {
                            var colName = col.Groups[1].Value;
                            if (!string.Equals(colName, "PRIMARY", StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(colName, "CLUSTERED", StringComparison.OrdinalIgnoreCase))
                            {
                                primaryKeys.Add(colName);
                            }
                        }
                    }
                    continue;
                }

                if (!line.StartsWith("["))
                {
                    continue;
                }

                var colMatch = Regex.Match(line, @"^\[(?<name>[^\]]+)\]\s+(?<type>\[[^\]]+\](\([^\)]*\))?)\s*(?<rest>.*)$", RegexOptions.IgnoreCase);
                if (!colMatch.Success)
                {
                    continue;
                }

                var name = colMatch.Groups["name"].Value;
                var type = colMatch.Groups["type"].Value;
                var rest = colMatch.Groups["rest"].Value;

                var sqliteType = MapSqlServerTypeToSqlite(type);
                var isIdentity = rest.Contains("IDENTITY", StringComparison.OrdinalIgnoreCase);
                var notNull = rest.Contains("NOT NULL", StringComparison.OrdinalIgnoreCase);

                if (isIdentity)
                {
                    identityPkColumn = name;
                    columns.Add($"[{name}] INTEGER PRIMARY KEY AUTOINCREMENT");
                    continue;
                }

                columns.Add($"[{name}] {sqliteType}{(notNull ? " NOT NULL" : string.Empty)}");
            }

            if (columns.Count == 0)
            {
                return string.Empty;
            }

            if (identityPkColumn == null && primaryKeys.Count > 0)
            {
                var uniqueKeys = primaryKeys.Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(k => $"[{k}]");
                columns.Add($"PRIMARY KEY ({string.Join(", ", uniqueKeys)})");
            }

            return $"CREATE TABLE IF NOT EXISTS {normalizedTableName} (\n    {string.Join(",\n    ", columns)}\n);";
        }

        private static string MapSqlServerTypeToSqlite(string sqlServerType)
        {
            var t = sqlServerType.Replace("[", string.Empty).Replace("]", string.Empty).Trim().ToLowerInvariant();

            if (t.StartsWith("bigint") || t.StartsWith("int") || t.StartsWith("smallint") || t.StartsWith("tinyint") || t.StartsWith("bit"))
                return "INTEGER";

            if (t.StartsWith("decimal") || t.StartsWith("numeric") || t.StartsWith("money") || t.StartsWith("smallmoney") || t.StartsWith("float") || t.StartsWith("real"))
                return "REAL";

            if (t.StartsWith("date") || t.StartsWith("datetime") || t.StartsWith("smalldatetime") || t.StartsWith("time"))
                return "TEXT";

            if (t.StartsWith("binary") || t.StartsWith("varbinary") || t.StartsWith("image") || t.StartsWith("rowversion") || t.StartsWith("timestamp"))
                return "BLOB";

            return "TEXT";
        }

        private static async Task CreateCoreCompatibilityTablesAsync(SqliteConnection conn, CancellationToken cancellationToken)
        {
            await ExecuteAsync(conn, @"
CREATE TABLE IF NOT EXISTS AppUser (
    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserName TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    UserFullName TEXT NOT NULL,
    Password TEXT NOT NULL,
    CompanyID INTEGER NOT NULL DEFAULT 1,
    CategoryID INTEGER NOT NULL DEFAULT 1,
    UserType INTEGER NOT NULL DEFAULT 1,
    UserImage TEXT NULL,
    CompanyName TEXT NULL,
    UserStatus INTEGER NOT NULL DEFAULT 1,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    LastLoginAt TEXT NULL,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS PasswordResetToken (
    TokenID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    Token TEXT NOT NULL UNIQUE,
    ExpiresAt TEXT NOT NULL,
    IsUsed INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS LoginAuditHistory (
    AuditID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NULL,
    UserName TEXT NULL,
    IsSuccess INTEGER NOT NULL,
    FailureReason TEXT NULL,
    IPAddress TEXT NULL,
    UserAgent TEXT NULL,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS MenuItems (
    MenuItemID INTEGER PRIMARY KEY AUTOINCREMENT,
    ParentID INTEGER NULL,
    LabelAR TEXT NOT NULL,
    LabelEN TEXT NOT NULL,
    DescriptionAR TEXT NULL,
    DescriptionEn TEXT NULL,
    Url TEXT NULL,
    Icon TEXT NULL,
    [Order] INTEGER NOT NULL DEFAULT 1,
    IsActive INTEGER NOT NULL DEFAULT 1,
    Permission TEXT NULL,
    Type TEXT NULL,
    CreatedBy INTEGER NOT NULL DEFAULT 1,
    UpdatedBy INTEGER NULL,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TEXT NULL
);
", cancellationToken);
        }

        private static async Task ExecuteAsync(SqliteConnection conn, string sql, CancellationToken cancellationToken)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private static async Task SeedUsersAsync(SqliteConnection conn, CancellationToken cancellationToken)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO AppUser (UserName, Email, UserFullName, Password, CompanyID, CategoryID, UserType, CompanyName, UserStatus, IsDeleted)
SELECT 'admin', 'admin@ebscore.local', 'System Administrator', 'admin123', 1, 1, 1, 'EBS Demo', 1, 0
WHERE NOT EXISTS (SELECT 1 FROM AppUser WHERE UserName = 'admin');";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private static async Task SeedMenuAsync(SqliteConnection conn, CancellationToken cancellationToken)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 1, NULL, 'لوحة التحكم', 'Dashboard', '/', 'fa-solid fa-square-poll-vertical', 1, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 1);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 2, NULL, 'استمرارية الأعمال', 'Business Continuity', NULL, 2, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 2);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 3, 2, 'تحليل أثر الأعمال', 'BIA', '/BCM/BIA', 1, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 3);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 4, 2, 'الحوادث', 'Incidents', '/BCM/Incidents', 2, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 4);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 5, 2, 'الموردون', 'Suppliers', '/BCM/Suppliers', 3, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 5);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 6, NULL, 'الضبط', 'Configuration', NULL, 3, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 6);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 7, 6, 'الموظفون', 'Employees', '/Config/Employees', 1, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 7);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 8, 6, 'العمليات', 'Processes', '/Config/Process', 2, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 8);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 9, NULL, 'الإشعارات', 'Notifications', '/Notification/S7SNotificationIndex', 4, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 9);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy, CreatedAt)
SELECT 10, NULL, 'الأمن', 'Security', '/Security/Roles', 5, 1, 1, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 10);
";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
