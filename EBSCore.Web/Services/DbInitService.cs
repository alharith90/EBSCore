using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace EBSCore.Web.Services;

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
        var provider = (_config["Database:Provider"] ?? "Sqlite").Trim();
        var cs = string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase)
            ? (_config.GetConnectionString("SqlServerConnection") ?? _config.GetConnectionString("DefaultConnection"))
            : _config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(cs))
        {
            _logger.LogWarning("Connection string is empty for provider {Provider}; DB initialization skipped.", provider);
            return;
        }

        await using var conn = CreateOpenConnection(provider, cs);
        var firstTime = await IsFirstTimeDatabaseAsync(conn, provider, cancellationToken);

        await EnsureMigrationHistoryTable(conn, provider, cancellationToken);

        var migrations = BuildMigrationPlan(provider).ToList();
        foreach (var migration in migrations)
        {
            await ApplyMigrationIfNeeded(conn, provider, migration, cancellationToken);
        }

        if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            await CompareSqliteAgainstBaselineAsync(conn, cancellationToken);
        }

        _logger.LogInformation("Database initialization completed. Provider={Provider}, FirstTime={FirstTime}, MigrationsChecked={Count}", provider, firstTime, migrations.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private DbConnection CreateOpenConnection(string provider, string connectionString)
    {
        if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            var sqlConn = new SqlConnection(connectionString);
            sqlConn.Open();
            return sqlConn;
        }

        var builder = new SqliteConnectionStringBuilder(connectionString);
        if (!Path.IsPathRooted(builder.DataSource))
        {
            builder.DataSource = Path.Combine(_env.ContentRootPath, builder.DataSource);
        }

        var directory = Path.GetDirectoryName(builder.DataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var sqliteConn = new SqliteConnection(builder.ToString());
        sqliteConn.Open();
        return sqliteConn;
    }

    private async Task<bool> IsFirstTimeDatabaseAsync(DbConnection conn, string provider, CancellationToken ct)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase)
            ? "SELECT COUNT(1) FROM sys.tables"
            : "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";

        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        return count == 0;
    }

    private IEnumerable<MigrationItem> BuildMigrationPlan(string provider)
    {
        var items = new List<MigrationItem>();
        var baseline = Path.Combine(_env.ContentRootPath, "AppData", "SQLSchemaAndData2403.sql");
        items.Add(new MigrationItem("0001_SQLSchemaAndData2403.sql", baseline, true));

        var providerFolder = Path.Combine(_env.ContentRootPath, "AppData", "Migrations", provider);
        if (Directory.Exists(providerFolder))
        {
            var files = Directory.GetFiles(providerFolder, "*.sql", SearchOption.TopDirectoryOnly)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                items.Add(new MigrationItem(Path.GetFileName(file), file, false));
            }
        }

        return items;
    }

    private async Task EnsureMigrationHistoryTable(DbConnection conn, string provider, CancellationToken ct)
    {
        var sql = string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase)
            ? @"IF OBJECT_ID('dbo.DbMigrationHistory','U') IS NULL
                CREATE TABLE dbo.DbMigrationHistory(
                    MigrationId INT IDENTITY(1,1) PRIMARY KEY,
                    MigrationFileName NVARCHAR(260) NOT NULL,
                    ProviderType NVARCHAR(32) NOT NULL,
                    VersionChecksum NVARCHAR(128) NULL,
                    MigrationVersion NVARCHAR(64) NULL,
                    Applied BIT NOT NULL,
                    AppliedOn DATETIME2 NULL,
                    ExecutionResult NVARCHAR(MAX) NULL
                );"
            : @"CREATE TABLE IF NOT EXISTS DbMigrationHistory(
                    MigrationId INTEGER PRIMARY KEY AUTOINCREMENT,
                    MigrationFileName TEXT NOT NULL,
                    ProviderType TEXT NOT NULL,
                    VersionChecksum TEXT NULL,
                    MigrationVersion TEXT NULL,
                    Applied INTEGER NOT NULL,
                    AppliedOn TEXT NULL,
                    ExecutionResult TEXT NULL
                );";

        await ExecuteNonQuery(conn, sql, ct);
        await EnsureHistoryExtendedColumns(conn, provider, ct);
    }

    private static async Task EnsureHistoryExtendedColumns(DbConnection conn, string provider, CancellationToken ct)
    {
        if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            await ExecuteNonQuery(conn, @"IF COL_LENGTH('dbo.DbMigrationHistory','MigrationVersion') IS NULL
                                          ALTER TABLE dbo.DbMigrationHistory ADD MigrationVersion NVARCHAR(64) NULL;", ct);
            return;
        }

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "PRAGMA table_info(DbMigrationHistory);";
        var hasMigrationVersion = false;
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                if (string.Equals(reader.GetString(1), "MigrationVersion", StringComparison.OrdinalIgnoreCase))
                {
                    hasMigrationVersion = true;
                    break;
                }
            }
        }

        if (!hasMigrationVersion)
        {
            await ExecuteNonQuery(conn, "ALTER TABLE DbMigrationHistory ADD COLUMN MigrationVersion TEXT NULL;", ct);
        }
    }

    private async Task ApplyMigrationIfNeeded(DbConnection conn, string provider, MigrationItem item, CancellationToken ct)
    {
        if (!File.Exists(item.Path))
        {
            _logger.LogWarning("Migration file not found: {Path}", item.Path);
            return;
        }

        var source = await ReadSqlFileAsync(item.Path, ct);
        var checksum = ComputeChecksum(source);
        var version = ExtractMigrationVersion(item.FileName);

        if (await IsApplied(conn, item.FileName, provider, checksum, ct))
        {
            return;
        }

        try
        {
            var script = BuildExecutableScript(source, provider);
            await ExecuteBatches(conn, script, provider, ct);
            await InsertHistory(conn, item.FileName, provider, checksum, version, true, "OK", ct);
            _logger.LogInformation("Applied migration {MigrationFileName}", item.FileName);
        }
        catch (Exception ex)
        {
            await InsertHistory(conn, item.FileName, provider, checksum, version, false, ex.Message, ct);
            _logger.LogError(ex, "Failed migration {MigrationFileName}", item.FileName);
            throw;
        }
    }

    private async Task<string> ReadSqlFileAsync(string path, CancellationToken ct)
    {
        try
        {
            return (await File.ReadAllTextAsync(path, Encoding.Unicode, ct)).Replace("\0", string.Empty);
        }
        catch
        {
            return (await File.ReadAllTextAsync(path, ct)).Replace("\0", string.Empty);
        }
    }

    private static string BuildExecutableScript(string source, string provider)
    {
        return string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase)
            ? BuildSqlServerScript(source)
            : BuildSqliteScript(source);
    }

    private static string BuildSqlServerScript(string source)
    {
        // Execute against current database connection. Ignore dump-specific USE statements.
        return Regex.Replace(source, @"^\s*USE\s+\[[^\]]+\]\s*$", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }

    private static string BuildSqliteScript(string source)
    {
        var convertedStatements = new List<string>();
        var batches = Regex.Split(source, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        foreach (var rawBatch in batches)
        {
            var batch = StripComments(rawBatch).Trim();
            if (string.IsNullOrWhiteSpace(batch))
            {
                continue;
            }

            if (IsNonExecutableInSqlite(batch))
            {
                continue;
            }

            if (TryConvertCreateTableStatements(batch, out var createTables))
            {
                foreach (var converted in createTables)
                {
                    if (!string.IsNullOrWhiteSpace(converted))
                    {
                        convertedStatements.Add(converted);
                    }
                }
                continue;
            }

            if (TryConvertCreateIndexStatements(batch, out var indexes))
            {
                convertedStatements.AddRange(indexes);
                continue;
            }

            if (TryConvertAlterTableAddColumn(batch, out var alter))
            {
                convertedStatements.Add(alter);
                continue;
            }

            if (ContainsDataManipulation(batch))
            {
                convertedStatements.Add(ConvertDataManipulationBatch(batch));
                continue;
            }

            if (TryConvertSimpleAlterTable(batch, out var alterTableSql))
            {
                convertedStatements.Add(alterTableSql);
                continue;
            }
        }

        return string.Join(";\n", convertedStatements);
    }

    private static bool ContainsDataManipulation(string batch)
    {
        return Regex.IsMatch(batch, @"\bINSERT(\s+INTO)?\s+\[", RegexOptions.IgnoreCase)
            || Regex.IsMatch(batch, @"\bUPDATE\b", RegexOptions.IgnoreCase)
            || Regex.IsMatch(batch, @"\bDELETE\s+FROM\b", RegexOptions.IgnoreCase);
    }

    private static bool IsNonExecutableInSqlite(string batch)
    {
        return batch.StartsWith("USE ", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("SET ANSI_", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("SET QUOTED_IDENTIFIER", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("CREATE PROCEDURE", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("CREATE PROC", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("ALTER PROCEDURE", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("CREATE FUNCTION", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("ALTER FUNCTION", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("EXEC ", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("PRINT ", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("/******", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("CREATE TRIGGER", StringComparison.OrdinalIgnoreCase)
            || batch.StartsWith("ALTER TRIGGER", StringComparison.OrdinalIgnoreCase);
    }

    private static string StripComments(string sql)
    {
        var withoutBlock = Regex.Replace(sql, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);
        var withoutLine = Regex.Replace(withoutBlock, @"^\s*--.*$", string.Empty, RegexOptions.Multiline);
        return withoutLine;
    }

    private static bool TryConvertCreateTableStatements(string batch, out IReadOnlyList<string> statements)
    {
        var matches = Regex.Matches(
            batch,
            @"CREATE\s+TABLE\s+(?:\[[^\]]+\]\.)?\[[^\]]+\]\s*\(",
            RegexOptions.IgnoreCase);
        if (matches.Count == 0)
        {
            statements = Array.Empty<string>();
            return false;
        }

        var list = new List<string>();
        foreach (Match m in matches)
        {
            var statement = ExtractCreateTableStatement(batch, m.Index);
            var converted = ConvertCreateTableBatch(statement);
            if (!string.IsNullOrWhiteSpace(converted))
            {
                list.Add(converted);
            }
        }

        statements = list;
        return statements.Count > 0;
    }

    private static string ExtractCreateTableStatement(string batch, int startIndex)
    {
        var openParen = batch.IndexOf('(', startIndex);
        if (openParen < 0)
        {
            return batch[startIndex..];
        }

        var depth = 0;
        for (var i = openParen; i < batch.Length; i++)
        {
            if (batch[i] == '(') depth++;
            else if (batch[i] == ')')
            {
                depth--;
                if (depth == 0)
                {
                    return batch[startIndex..(i + 1)];
                }
            }
        }

        return batch[startIndex..];
    }

    private static bool TryConvertCreateIndexStatements(string batch, out IReadOnlyList<string> statements)
    {
        var normalizedBatch = NormalizeSchemaQualifiedNames(batch);
        var matches = Regex.Matches(
            normalizedBatch,
            @"CREATE\s+(UNIQUE\s+)?(NONCLUSTERED\s+|CLUSTERED\s+)?INDEX\s+\[[^\]]+\]\s+ON\s+\[[^\]]+\]\s*\([^)]+\)",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (matches.Count == 0)
        {
            statements = Array.Empty<string>();
            return false;
        }

        statements = matches
            .Select(m => Regex.Replace(m.Value, @"\b(CLUSTERED|NONCLUSTERED)\b", string.Empty, RegexOptions.IgnoreCase))
            .Select(m => Regex.Replace(m, @"\s+WITH\s*\(.*?\)", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline))
            .Select(m => Regex.Replace(m, @"\s+ON\s+\[[^\]]+\]\s*$", string.Empty, RegexOptions.IgnoreCase))
            .Select(m => Regex.Replace(m, @"\s+", " ").Trim())
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .ToList();
        return statements.Count > 0;
    }

    private static bool TryConvertAlterTableAddColumn(string batch, out string statement)
    {
        var match = Regex.Match(batch, @"ALTER\s+TABLE\s+(?:\[(?<schema>[^\]]+)\]\.)?\[(?<table>[^\]]+)\]\s+ADD\s+\[(?<col>[^\]]+)\]\s+(?<type>[a-zA-Z0-9]+)(\([^)]+\))?(?<rest>[^;]*)", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            statement = string.Empty;
            return false;
        }

        var schema = match.Groups["schema"].Success ? match.Groups["schema"].Value : "dbo";
        var table = match.Groups["table"].Value;
        var column = match.Groups["col"].Value;
        var type = match.Groups["type"].Value;
        var rest = match.Groups["rest"].Value;
        var tableName = string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase) ? table : $"{schema}_{table}";
        var nullable = rest.Contains("NOT NULL", StringComparison.OrdinalIgnoreCase) ? " NOT NULL" : string.Empty;

        statement = $"ALTER TABLE [{tableName}] ADD COLUMN [{column}] {MapType(type)}{nullable}";
        return true;
    }

    private static string ConvertCreateTableBatch(string batch)
    {
        var openParen = batch.IndexOf('(');
        if (openParen < 0)
        {
            return string.Empty;
        }

        var header = batch[..openParen].Trim();
        var tableName = NormalizeSchemaQualifiedNames(header).Replace("CREATE TABLE", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

        var body = batch[openParen..];
        var full = $"CREATE TABLE {tableName} {body}";
        return ConvertSqlServerCreateTableToSqlite(full);
    }

    private static string ConvertDataManipulationBatch(string batch)
    {
        var converted = NormalizeSchemaQualifiedNames(batch);
        converted = Regex.Replace(converted, @"\bN'", "'", RegexOptions.IgnoreCase);
        converted = Regex.Replace(converted, @"\bGETDATE\s*\(\s*\)", "CURRENT_TIMESTAMP", RegexOptions.IgnoreCase);
        converted = Regex.Replace(converted, @"\bGETUTCDATE\s*\(\s*\)", "CURRENT_TIMESTAMP", RegexOptions.IgnoreCase);
        converted = Regex.Replace(converted, @"\bISNULL\s*\(", "IFNULL(", RegexOptions.IgnoreCase);
        converted = Regex.Replace(converted, @"\bCONVERT\s*\(\s*\w+\s*,\s*([^\)]+)\)", "$1", RegexOptions.IgnoreCase);
        converted = Regex.Replace(converted, @"CAST\s*\(\s*N?'([^']*)'\s+AS\s+DateTime2?\s*\)", "'$1'", RegexOptions.IgnoreCase);
        converted = Regex.Replace(converted, @"CAST\s*\(\s*N?'([^']*)'\s+AS\s+Date\s*\)", "'$1'", RegexOptions.IgnoreCase);
        converted = Regex.Replace(converted, @"CAST\s*\(\s*N?'([^']*)'\s+AS\s+Time\s*\)", "'$1'", RegexOptions.IgnoreCase);
        converted = Regex.Replace(converted, @"\bWITH\s*\(.*?\)", string.Empty, RegexOptions.IgnoreCase);
        return converted.Trim();
    }

    private static bool TryConvertSimpleAlterTable(string batch, out string statement)
    {
        var match = Regex.Match(batch, @"ALTER\s+TABLE\s+(?:\[(?<schema>[^\]]+)\]\.)?\[(?<table>[^\]]+)\]\s+DROP\s+CONSTRAINT\s+\[[^\]]+\]", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            statement = $"-- Skipped for SQLite compatibility: {NormalizeSchemaQualifiedNames(batch)}";
            return true;
        }

        statement = string.Empty;
        return false;
    }

    private static string NormalizeSchemaQualifiedNames(string sql)
    {
        sql = Regex.Replace(sql, @"\[(?<schema>[^\]]+)\]\.\[(?<name>[^\]]+)\]", m =>
        {
            var schema = m.Groups["schema"].Value;
            var name = m.Groups["name"].Value;
            return string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase)
                ? $"[{name}]"
                : $"[{schema}_{name}]";
        }, RegexOptions.IgnoreCase);

        // Handle unbracketed two-part names e.g. dbo.Table or HangFire.Job
        sql = Regex.Replace(sql, @"(?<![\w\]])(?<schema>[A-Za-z_][\w]*)\.(?<name>[A-Za-z_][\w]*)(?![\w\[])", m =>
        {
            var schema = m.Groups["schema"].Value;
            var name = m.Groups["name"].Value;
            return string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase)
                ? $"[{name}]"
                : $"[{schema}_{name}]";
        });

        return sql;
    }

    private static string ConvertSqlServerCreateTableToSqlite(string script)
    {
        var marker = "CREATE TABLE";
        var start = script.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
        {
            return string.Empty;
        }

        var openParen = script.IndexOf('(', start);
        if (openParen < 0)
        {
            return string.Empty;
        }

        var header = script[start..openParen].Trim();
        var tableName = header.Replace("CREATE TABLE", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

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

        if (closeParen < 0)
        {
            return string.Empty;
        }

        var body = script[(openParen + 1)..closeParen];
        var lines = body.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        var columns = new List<string>();
        var primaryKeys = new List<string>();

        foreach (var raw in lines)
        {
            var line = raw.Trim().TrimEnd(',');

            if (line.StartsWith("CONSTRAINT", StringComparison.OrdinalIgnoreCase))
            {
                if (line.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Match col in Regex.Matches(line, "\\[([^\\]]+)\\]"))
                    {
                        var c = col.Groups[1].Value;
                        if (!string.Equals(c, "PRIMARY", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(c, "CLUSTERED", StringComparison.OrdinalIgnoreCase))
                        {
                            primaryKeys.Add($"[{c}]");
                        }
                    }
                }

                continue;
            }

            var m = Regex.Match(line, @"^\[(?<col>[^\]]+)\]\s+(?<type>[a-zA-Z0-9]+)(\([^)]+\))?(?<rest>.*)$", RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                continue;
            }

            var col = m.Groups["col"].Value;
            var type = m.Groups["type"].Value;
            var rest = m.Groups["rest"].Value;

            var nullable = rest.Contains("NOT NULL", StringComparison.OrdinalIgnoreCase) ? " NOT NULL" : string.Empty;
            var autoIdentity = rest.Contains("IDENTITY", StringComparison.OrdinalIgnoreCase)
                ? " PRIMARY KEY AUTOINCREMENT"
                : string.Empty;

            if (!string.IsNullOrEmpty(autoIdentity))
            {
                columns.Add($"[{col}] INTEGER{autoIdentity}");
            }
            else
            {
                columns.Add($"[{col}] {MapType(type)}{nullable}");
            }
        }

        if (columns.Count == 0)
        {
            return string.Empty;
        }

        if (primaryKeys.Count > 0 && !columns.Any(c => c.Contains("PRIMARY KEY AUTOINCREMENT", StringComparison.OrdinalIgnoreCase)))
        {
            columns.Add($"PRIMARY KEY ({string.Join(", ", primaryKeys.Distinct())})");
        }

        return $"CREATE TABLE IF NOT EXISTS {tableName} (\n  {string.Join(",\n  ", columns)}\n)";
    }

    private static string MapType(string sqlServerType)
    {
        return sqlServerType.ToLowerInvariant() switch
        {
            "bigint" or "int" or "smallint" or "tinyint" or "bit" => "INTEGER",
            "decimal" or "numeric" or "money" or "smallmoney" or "float" or "real" => "REAL",
            "datetime" or "datetime2" or "smalldatetime" or "date" or "time" => "TEXT",
            "varbinary" or "binary" or "image" => "BLOB",
            _ => "TEXT"
        };
    }

    private async Task CompareSqliteAgainstBaselineAsync(DbConnection conn, CancellationToken ct)
    {
        var baseline = Path.Combine(_env.ContentRootPath, "AppData", "SQLSchemaAndData2403.sql");
        if (!File.Exists(baseline))
        {
            return;
        }

        var source = await ReadSqlFileAsync(baseline, ct);
        var expected = Regex.Matches(source, @"CREATE\s+TABLE\s+\[(?<schema>[^\]]+)\]\.\[(?<name>[^\]]+)\]", RegexOptions.IgnoreCase)
            .Select(m =>
            {
                var s = m.Groups["schema"].Value;
                var n = m.Groups["name"].Value;
                return string.Equals(s, "dbo", StringComparison.OrdinalIgnoreCase) ? n : $"{s}_{n}";
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var expectedColumns = ExtractExpectedColumns(source);
        var expectedSeedCounts = ExtractExpectedSeedCounts(source);

        var actual = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var actualColumns = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        var actualSeedCounts = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                actual.Add(reader.GetString(0));
            }
        }

        foreach (var table in actual)
        {
            actualColumns[table] = await GetActualColumns(conn, table, ct);
            actualSeedCounts[table] = await GetTableCount(conn, table, ct);
        }

        var missing = expected.Except(actual, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        var missingColumns = new List<string>();
        foreach (var kv in expectedColumns)
        {
            if (!actualColumns.TryGetValue(kv.Key, out var colSet))
            {
                continue;
            }

            var colMissing = kv.Value.Except(colSet, StringComparer.OrdinalIgnoreCase).ToList();
            if (colMissing.Count > 0)
            {
                missingColumns.Add($"{kv.Key}: {string.Join(", ", colMissing)}");
            }
        }

        var seedMismatches = expectedSeedCounts
            .Where(kv => actualSeedCounts.TryGetValue(kv.Key, out var actualCount) && actualCount < kv.Value)
            .Select(kv => $"{kv.Key}: expected-at-least={kv.Value}, actual={actualSeedCounts.GetValueOrDefault(kv.Key)}")
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _logger.LogInformation("SQLite baseline comparison: expectedTables={Expected}, actualTables={Actual}, missingTables={Missing}", expected.Count, actual.Count, missing.Count);
        if (missing.Count > 0)
        {
            _logger.LogWarning("SQLite missing tables compared to SQLSchemaAndData2403.sql: {Tables}", string.Join(", ", missing.Take(50)));
        }

        await WriteGapReportAsync(expected, actual, missing, missingColumns, seedMismatches, ct);
    }

    private async Task<bool> IsApplied(DbConnection conn, string fileName, string provider, string checksum, CancellationToken ct)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT COUNT(1) FROM DbMigrationHistory
                            WHERE MigrationFileName = @f AND ProviderType = @p AND VersionChecksum = @c AND Applied = 1";
        AddParam(cmd, "@f", fileName);
        AddParam(cmd, "@p", provider);
        AddParam(cmd, "@c", checksum);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) > 0;
    }

    private async Task InsertHistory(DbConnection conn, string fileName, string provider, string checksum, string version, bool applied, string result, CancellationToken ct)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO DbMigrationHistory(MigrationFileName, ProviderType, VersionChecksum, MigrationVersion, Applied, AppliedOn, ExecutionResult)
                            VALUES(@f,@p,@c,@v,@a,@d,@r);";
        AddParam(cmd, "@f", fileName);
        AddParam(cmd, "@p", provider);
        AddParam(cmd, "@c", checksum);
        AddParam(cmd, "@v", version);
        AddParam(cmd, "@a", applied ? 1 : 0);
        AddParam(cmd, "@d", DateTime.UtcNow.ToString("O"));
        AddParam(cmd, "@r", result);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static string ExtractMigrationVersion(string fileName)
    {
        var m = Regex.Match(fileName, @"^(?<v>\d+)");
        return m.Success ? m.Groups["v"].Value : "baseline";
    }

    private static Dictionary<string, HashSet<string>> ExtractExpectedColumns(string source)
    {
        var output = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        var matches = Regex.Matches(source, @"CREATE\s+TABLE\s+\[(?<schema>[^\]]+)\]\.\[(?<name>[^\]]+)\]\s*\((?<body>.*?)\)\s*GO", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        foreach (Match m in matches)
        {
            var schema = m.Groups["schema"].Value;
            var name = m.Groups["name"].Value;
            var table = string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase) ? name : $"{schema}_{name}";
            var body = m.Groups["body"].Value;
            var cols = Regex.Matches(body, @"\[(?<col>[^\]]+)\]\s+\[", RegexOptions.IgnoreCase)
                .Select(x => x.Groups["col"].Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            output[table] = cols;
        }

        return output;
    }

    private static Dictionary<string, long> ExtractExpectedSeedCounts(string source)
    {
        var output = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        var matches = Regex.Matches(source, @"INSERT(\s+INTO)?\s+\[(?<schema>[^\]]+)\]\.\[(?<name>[^\]]+)\]", RegexOptions.IgnoreCase);
        foreach (Match m in matches)
        {
            var schema = m.Groups["schema"].Value;
            var name = m.Groups["name"].Value;
            var table = string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase) ? name : $"{schema}_{name}";
            output[table] = output.GetValueOrDefault(table) + 1;
        }

        return output;
    }

    private static async Task<HashSet<string>> GetActualColumns(DbConnection conn, string tableName, CancellationToken ct)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info([{tableName.Replace("]", "]]", StringComparison.Ordinal)}]);";
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            set.Add(reader.GetString(1));
        }

        return set;
    }

    private static async Task<long> GetTableCount(DbConnection conn, string tableName, CancellationToken ct)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(1) FROM [{tableName.Replace("]", "]]", StringComparison.Ordinal)}]";
        return Convert.ToInt64(await cmd.ExecuteScalarAsync(ct), CultureInfo.InvariantCulture);
    }

    private async Task WriteGapReportAsync(
        HashSet<string> expected,
        HashSet<string> actual,
        List<string> missingTables,
        List<string> missingColumns,
        List<string> seedMismatches,
        CancellationToken ct)
    {
        var folder = Path.Combine(_env.ContentRootPath, "Logs");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, "DbGapReport-Sqlite.txt");

        var previous = await TryReadPreviousGapReportAsync(path, ct);
        var restoredTables = previous.MissingTables.Except(missingTables, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        var restoredColumns = previous.MissingColumns.Except(missingColumns, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        var restoredSeed = previous.SeedMismatches.Except(seedMismatches, StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();

        var lines = new List<string>
        {
            $"GeneratedOnUtc: {DateTime.UtcNow:O}",
            $"SchemaSource: AppData/SQLSchemaAndData2403.sql",
            $"ExpectedTables: {expected.Count}",
            $"ActualTables: {actual.Count}",
            $"MissingTables: {missingTables.Count}",
            $"MissingColumnsSets: {missingColumns.Count}",
            $"SeedCountMismatches: {seedMismatches.Count}",
            $"PreviouslyMissingTables: {previous.MissingTables.Count}",
            $"PreviouslyMissingColumnsSets: {previous.MissingColumns.Count}",
            $"PreviouslySeedCountMismatches: {previous.SeedMismatches.Count}",
            $"RestoredTablesSincePreviousRun: {restoredTables.Count}",
            $"RestoredColumnsSincePreviousRun: {restoredColumns.Count}",
            $"RestoredSeedCountsSincePreviousRun: {restoredSeed.Count}",
            string.Empty,
            "[Restored Since Previous Run - Tables]"
        };
        lines.AddRange(restoredTables);
        lines.Add(string.Empty);
        lines.Add("[Restored Since Previous Run - Columns]");
        lines.AddRange(restoredColumns);
        lines.Add(string.Empty);
        lines.Add("[Restored Since Previous Run - Seed Count]");
        lines.AddRange(restoredSeed);
        lines.Add(string.Empty);
        lines.Add("[Missing Tables]");
        lines.AddRange(missingTables);
        lines.Add(string.Empty);
        lines.Add("[Missing Columns]");
        lines.AddRange(missingColumns);
        lines.Add(string.Empty);
        lines.Add("[Seed Count Mismatches]");
        lines.AddRange(seedMismatches);

        await File.WriteAllLinesAsync(path, lines, ct);
    }

    private static async Task<GapReportSnapshot> TryReadPreviousGapReportAsync(string path, CancellationToken ct)
    {
        if (!File.Exists(path))
        {
            return new GapReportSnapshot(new List<string>(), new List<string>(), new List<string>());
        }

        var lines = await File.ReadAllLinesAsync(path, ct);
        return new GapReportSnapshot(
            ReadSection(lines, "[Missing Tables]"),
            ReadSection(lines, "[Missing Columns]"),
            ReadSection(lines, "[Seed Count Mismatches]"));
    }

    private static List<string> ReadSection(string[] lines, string header)
    {
        var list = new List<string>();
        var start = Array.FindIndex(lines, x => string.Equals(x.Trim(), header, StringComparison.OrdinalIgnoreCase));
        if (start < 0)
        {
            return list;
        }

        for (var i = start + 1; i < lines.Length; i++)
        {
            var value = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                break;
            }

            if (value.StartsWith("[", StringComparison.Ordinal))
            {
                break;
            }

            list.Add(value);
        }

        return list;
    }

    private static async Task ExecuteBatches(DbConnection conn, string script, string provider, CancellationToken ct)
    {
        var batches = string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase)
            ? Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
            : script.Split(';');

        foreach (var rawBatch in batches)
        {
            var batch = rawBatch.Trim();
            if (string.IsNullOrWhiteSpace(batch))
            {
                continue;
            }

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = batch;
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }

    private static void AddParam(DbCommand cmd, string name, object? value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(p);
    }

    private static async Task ExecuteNonQuery(DbConnection conn, string sql, CancellationToken ct)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static string ComputeChecksum(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }

    private sealed record MigrationItem(string FileName, string Path, bool IsBaseline);
    private sealed record GapReportSnapshot(List<string> MissingTables, List<string> MissingColumns, List<string> SeedMismatches);
}
