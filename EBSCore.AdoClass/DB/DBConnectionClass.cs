using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace EBSCore.AdoClass
{
    public class DBConnectionClass
    {
        private readonly string _connectionString;
        private readonly string _provider;

        public DBConnectionClass(IConfiguration configuration)
        {
            _provider = (configuration["Database:Provider"] ?? "Sqlite").Trim();
            var sqlServerConnection = configuration.GetConnectionString("SqlServerConnection");
            _connectionString = UseProviderSqlServer(_provider)
                ? (sqlServerConnection ?? configuration.GetConnectionString("DefaultConnection"))
                : configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("A valid connection string is required for selected database provider.");
            }
        }

        private bool UseSqlServer => UseProviderSqlServer(_provider);

        private static bool UseProviderSqlServer(string provider)
            => string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase);

        public int ExecuteNonQuery(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            return ExecuteStoredProcedureNonQuery(storedProcedureName, ToDictionary(DBFieldsArrayList));
        }

        public object ExecuteScalar(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            var parameters = ToDictionary(DBFieldsArrayList);
            if (UseSqlServer)
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(storedProcedureName, conn) { CommandType = CommandType.StoredProcedure };
                AddParameters(cmd, parameters);
                return cmd.ExecuteScalar() ?? DBNull.Value;
            }

            using var sqliteConn = CreateSqliteConnection();
            sqliteConn.Open();
            using var sqliteCmd = sqliteConn.CreateCommand();
            sqliteCmd.CommandText = storedProcedureName;
            AddParameters(sqliteCmd, parameters);
            return sqliteCmd.ExecuteScalar() ?? DBNull.Value;
        }

        public IDataReader ExecuteReader(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            var parameters = ToDictionary(DBFieldsArrayList);
            if (UseSqlServer)
            {
                var conn = new SqlConnection(_connectionString);
                conn.Open();
                var cmd = new SqlCommand(storedProcedureName, conn) { CommandType = CommandType.StoredProcedure };
                AddParameters(cmd, parameters);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }

            var sqliteConn = CreateSqliteConnection();
            sqliteConn.Open();
            var sqliteCmd = sqliteConn.CreateCommand();
            sqliteCmd.CommandText = storedProcedureName;
            AddParameters(sqliteCmd, parameters);
            return sqliteCmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public DataSet FillDataset(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            if (UseSqlServer)
            {
                return FillSqlServerDataset(storedProcedureName, ToDictionary(DBFieldsArrayList));
            }

            return ExecuteStoredProcedureFillDataset(storedProcedureName, ToDictionary(DBFieldsArrayList));
        }

        public DataSet FillDataset(string query)
        {
            var dataSet = new DataSet();
            var table = new DataTable();

            using var conn = CreateDbConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = query;
            using var reader = cmd.ExecuteReader();
            table.Load(reader);
            dataSet.Tables.Add(table);

            return dataSet;
        }

        public System.Xml.XmlReader? ExecuteXmlReader(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            return null;
        }

        public string ConnectionString => _connectionString;

        public IDbConnection DBSqlConnection => CreateDbConnection();

        private DbConnection CreateDbConnection()
        {
            return UseSqlServer
                ? new SqlConnection(_connectionString)
                : CreateSqliteConnection();
        }

        private SqliteConnection CreateSqliteConnection() => new(_connectionString);

        private static Dictionary<string, object?> ToDictionary(ArrayList fields)
        {
            var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in fields)
            {
                if (item is TableField field)
                {
                    result[field.Name] = field.Value;
                }
            }

            return result;
        }

        private static void AddParameters(DbCommand command, Dictionary<string, object?> parameters)
        {
            foreach (var item in parameters)
            {
                var parameterName = item.Key.StartsWith("@", StringComparison.Ordinal) ? item.Key : $"@{item.Key}";
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = item.Value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        private static void AddParameters(SqlCommand command, Dictionary<string, object?> parameters)
        {
            foreach (var item in parameters)
            {
                var parameterName = item.Key.StartsWith("@", StringComparison.Ordinal) ? item.Key : $"@{item.Key}";
                command.Parameters.AddWithValue(parameterName, item.Value ?? DBNull.Value);
            }
        }

        private DataSet FillSqlServerDataset(string procedureName, Dictionary<string, object?> parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procedureName, conn) { CommandType = CommandType.StoredProcedure };
            AddParameters(cmd, parameters);
            using var adapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            adapter.Fill(ds);
            if (ds.Tables.Count == 0)
            {
                ds.Tables.Add(new DataTable());
            }

            return ds;
        }

        private DataSet ExecuteStoredProcedureFillDataset(string procedureName, Dictionary<string, object?> parameters)
        {
            switch (procedureName)
            {
                case "S7SUserAuthSP":
                    return ExecuteUserAuthFill(parameters);
                case "UserSP":
                    return ExecuteUserSpFill(parameters);
                case "MenuItemsSP":
                    return ExecuteMenuItemsFill(parameters);
                case "sec.SecuritySP":
                    return ExecuteSecurityFill(parameters);
                default:
                    return ExecuteGenericFillDataset(procedureName, parameters);
            }
        }

        private int ExecuteStoredProcedureNonQuery(string procedureName, Dictionary<string, object?> parameters)
        {
            if (UseSqlServer)
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(procedureName, conn) { CommandType = CommandType.StoredProcedure };
                AddParameters(cmd, parameters);
                return cmd.ExecuteNonQuery();
            }

            switch (procedureName)
            {
                case "S7SUserAuthSP":
                    return ExecuteUserAuthNonQuery(parameters);
                default:
                    return ExecuteGenericNonQuery(procedureName, parameters);
            }
        }

        // SQLite compatibility implementation below
        private DataSet ExecuteUserSpFill(Dictionary<string, object?> parameters)
        {
            var operation = GetParameter(parameters, "Operation");
            if (!string.Equals(operation, "LoginWithNoHistory", StringComparison.OrdinalIgnoreCase))
            {
                return EmptyDataSet();
            }

            var userName = GetParameter(parameters, "UserName");
            var password = GetParameter(parameters, "Password");

            const string sql = @"SELECT UserID, Email, UserFullName, CompanyID, CategoryID, UserType, UserName,
                                        COALESCE(UserImage, '') AS UserImage,
                                        COALESCE(CompanyName, 'EBS Demo') AS CompanyName
                                 FROM AppUser
                                 WHERE UserName = @UserName AND Password = @Password AND UserStatus = 1 AND IsDeleted = 0
                                 LIMIT 1;";

            return QueryToDataSet(sql, new Dictionary<string, object?>
            {
                ["@UserName"] = userName,
                ["@Password"] = password
            });
        }

        private DataSet ExecuteMenuItemsFill(Dictionary<string, object?> parameters)
        {
            var operation = GetParameter(parameters, "Operation");
            if (!string.Equals(operation, "rtvMenuItems", StringComparison.OrdinalIgnoreCase))
            {
                return EmptyDataSet();
            }

            const string sql = @"SELECT MenuItemID, ParentID, LabelAR, LabelEN, DescriptionAR, DescriptionEn, Url, Icon,
                                        [Order], IsActive, Permission, Type, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
                                 FROM MenuItems
                                 WHERE IsActive = 1
                                 ORDER BY [Order], MenuItemID;";
            return QueryToDataSet(sql, null);
        }

        private DataSet ExecuteSecurityFill(Dictionary<string, object?> parameters)
        {
            var operation = GetParameter(parameters, "Operation");
            if (!string.Equals(operation, "rtvUserEffectivePermissions", StringComparison.OrdinalIgnoreCase))
            {
                return EmptyDataSet();
            }

            var table = new DataTable();
            table.Columns.Add("MenuCode");
            table.Columns.Add("ActionCode");
            table.Columns.Add("IsAllowed", typeof(bool));
            table.Rows.Add("*", "*", true);

            var ds = new DataSet();
            ds.Tables.Add(table);
            return ds;
        }

        private DataSet ExecuteUserAuthFill(Dictionary<string, object?> parameters)
        {
            var operation = GetParameter(parameters, "Operation");

            return operation switch
            {
                "Login" => UserLogin(parameters),
                "GetUserByUsername" => QueryToDataSet("SELECT UserID, UserName, Email FROM AppUser WHERE UserName = @v AND IsDeleted = 0 LIMIT 1;", new Dictionary<string, object?> { ["@v"] = GetParameter(parameters, "UserName") }),
                "GetUserByEmail" => QueryToDataSet("SELECT UserID, UserName, Email FROM AppUser WHERE Email = @v AND IsDeleted = 0 LIMIT 1;", new Dictionary<string, object?> { ["@v"] = GetParameter(parameters, "Email") }),
                "CreateResetToken" => CreateResetToken(parameters),
                "ValidateResetToken" => ValidateResetToken(parameters),
                _ => EmptyDataSet()
            };
        }

        private int ExecuteUserAuthNonQuery(Dictionary<string, object?> parameters)
        {
            var operation = GetParameter(parameters, "Operation");

            return operation switch
            {
                "UpdateLastLogin" => ExecuteSql("UPDATE AppUser SET LastLoginAt = CURRENT_TIMESTAMP WHERE UserID = @UserID", new Dictionary<string, object?> { ["@UserID"] = GetParameter(parameters, "UserID") }),
                "ResetPassword" => ResetPassword(parameters),
                _ => 0
            };
        }

        private DataSet UserLogin(Dictionary<string, object?> parameters)
        {
            var userName = GetParameter(parameters, "UserName");
            var password = GetParameter(parameters, "Password");

            const string sql = @"SELECT UserID, Email, UserFullName, CompanyID, CategoryID, UserType, UserName,
                                        FailedLoginAttempts, LockUntil, LastLoginAt, Password, UserStatus
                                 FROM AppUser
                                 WHERE (UserName = @UserName OR Email = @UserName) AND IsDeleted = 0
                                 LIMIT 1;";

            var ds = QueryToDataSet(sql, new Dictionary<string, object?> { ["@UserName"] = userName });
            var outTable = new DataTable();
            outTable.Columns.Add("IsAuthenticated", typeof(bool));
            outTable.Columns.Add("Reason", typeof(string));
            outTable.Columns.Add("UserID", typeof(long));
            outTable.Columns.Add("Email", typeof(string));
            outTable.Columns.Add("UserFullName", typeof(string));
            outTable.Columns.Add("CompanyID", typeof(long));
            outTable.Columns.Add("CategoryID", typeof(long));
            outTable.Columns.Add("UserType", typeof(int));
            outTable.Columns.Add("UserName", typeof(string));
            outTable.Columns.Add("LastLoginAt", typeof(string));

            if (ds.Tables[0].Rows.Count == 0)
            {
                outTable.Rows.Add(false, "InvalidCredentials", DBNull.Value, "", "", 0, 0, 0, "", "");
            }
            else
            {
                var row = ds.Tables[0].Rows[0];
                var storedPassword = row["Password"]?.ToString() ?? string.Empty;
                var isActive = Convert.ToInt32(row["UserStatus"]) == 1;
                var ok = isActive && string.Equals(storedPassword, password, StringComparison.Ordinal);
                outTable.Rows.Add(ok, ok ? "" : "InvalidCredentials", row["UserID"], row["Email"], row["UserFullName"], row["CompanyID"], row["CategoryID"], row["UserType"], row["UserName"], row["LastLoginAt"]?.ToString() ?? "");
            }

            var result = new DataSet();
            result.Tables.Add(outTable);
            return result;
        }

        private DataSet CreateResetToken(Dictionary<string, object?> parameters)
        {
            var token = Guid.NewGuid().ToString();
            var userId = GetParameter(parameters, "UserID");
            var expiryMins = int.TryParse(GetParameter(parameters, "TokenExpiryMinutes"), out var mins) ? mins : 60;

            ExecuteSql(@"INSERT INTO PasswordResetToken (UserID, Token, ExpiresAt, IsUsed, CreatedAt)
                         VALUES (@UserID, @Token, datetime('now', @Offset), 0, CURRENT_TIMESTAMP);",
                new Dictionary<string, object?>
                {
                    ["@UserID"] = userId,
                    ["@Token"] = token,
                    ["@Offset"] = $"+{expiryMins} minutes"
                });

            var table = new DataTable();
            table.Columns.Add("Token", typeof(string));
            table.Rows.Add(token);
            var ds = new DataSet();
            ds.Tables.Add(table);
            return ds;
        }

        private DataSet ValidateResetToken(Dictionary<string, object?> parameters)
        {
            const string sql = @"SELECT t.UserID, u.UserName, u.Email, t.Token
                                 FROM PasswordResetToken t
                                 JOIN AppUser u ON u.UserID = t.UserID
                                 WHERE t.Token = @Token
                                   AND t.IsUsed = 0
                                   AND datetime(t.ExpiresAt) >= datetime('now')
                                 ORDER BY t.TokenID DESC
                                 LIMIT 1;";

            return QueryToDataSet(sql, new Dictionary<string, object?> { ["@Token"] = GetParameter(parameters, "Token") });
        }

        private int ResetPassword(Dictionary<string, object?> parameters)
        {
            var userId = GetParameter(parameters, "UserID");
            var token = GetParameter(parameters, "Token");
            var newPassword = GetParameter(parameters, "NewPassword");

            ExecuteSql("UPDATE AppUser SET Password = @Password WHERE UserID = @UserID", new Dictionary<string, object?> { ["@Password"] = newPassword, ["@UserID"] = userId });
            return ExecuteSql("UPDATE PasswordResetToken SET IsUsed = 1 WHERE Token = @Token AND UserID = @UserID", new Dictionary<string, object?> { ["@Token"] = token, ["@UserID"] = userId });
        }

        private DataSet QueryToDataSet(string sql, Dictionary<string, object?>? parameters)
        {
            var table = new DataTable();
            using var conn = CreateSqliteConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                }
            }

            using var reader = cmd.ExecuteReader();
            table.Load(reader);

            var ds = new DataSet();
            ds.Tables.Add(table);
            return ds;
        }

        private int ExecuteSql(string sql, Dictionary<string, object?>? parameters)
        {
            using var conn = CreateSqliteConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                }
            }
            return cmd.ExecuteNonQuery();
        }

        private DataSet ExecuteGenericFillDataset(string procedureName, Dictionary<string, object?> parameters)
        {
            var operation = GetParameter(parameters, "Operation");
            var table = ResolveTableName(procedureName, operation);
            if (string.IsNullOrWhiteSpace(table))
            {
                return EmptyDataSet();
            }

            var clauses = new List<string>();
            var sqlParams = new Dictionary<string, object?>();
            foreach (var item in parameters)
            {
                if (string.Equals(item.Key, "Operation", StringComparison.OrdinalIgnoreCase) || item.Value == null || string.IsNullOrWhiteSpace(item.Value.ToString()))
                {
                    continue;
                }

                if (item.Key.EndsWith("ID", StringComparison.OrdinalIgnoreCase) && !string.Equals(item.Key, "CompanyID", StringComparison.OrdinalIgnoreCase))
                {
                    var pn = $"@{item.Key}";
                    clauses.Add($"[{item.Key}] = {pn}");
                    sqlParams[pn] = item.Value;
                }
            }

            var where = clauses.Count > 0 ? $" WHERE {string.Join(" AND ", clauses)}" : string.Empty;
            return QueryToDataSet($"SELECT * FROM [{table}] {where} LIMIT 500", sqlParams);
        }

        private int ExecuteGenericNonQuery(string procedureName, Dictionary<string, object?> parameters)
        {
            var operation = GetParameter(parameters, "Operation");
            var table = ResolveTableName(procedureName, operation);
            if (string.IsNullOrWhiteSpace(table))
            {
                return 0;
            }

            if (operation.StartsWith("Delete", StringComparison.OrdinalIgnoreCase))
            {
                var idField = parameters.Keys.FirstOrDefault(k => k.EndsWith("ID", StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrWhiteSpace(idField) || string.IsNullOrWhiteSpace(GetParameter(parameters, idField)))
                {
                    return 0;
                }

                return ExecuteSql($"DELETE FROM [{table}] WHERE [{idField}] = @{idField}", new Dictionary<string, object?> { [$"@{idField}"] = parameters[idField] });
            }

            if (operation.StartsWith("Save", StringComparison.OrdinalIgnoreCase) || operation.StartsWith("Add", StringComparison.OrdinalIgnoreCase) || operation.StartsWith("Update", StringComparison.OrdinalIgnoreCase))
            {
                var values = parameters.Where(p => !string.Equals(p.Key, "Operation", StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.Value != null && !string.IsNullOrWhiteSpace(p.Value.ToString()))
                    .ToDictionary(p => p.Key, p => p.Value);

                var idField = values.Keys.FirstOrDefault(k => k.EndsWith("ID", StringComparison.OrdinalIgnoreCase));
                var hasId = !string.IsNullOrWhiteSpace(idField) && !string.IsNullOrWhiteSpace(values[idField]?.ToString());

                if (hasId)
                {
                    var setParts = values.Where(v => !string.Equals(v.Key, idField, StringComparison.OrdinalIgnoreCase)).Select(v => $"[{v.Key}] = @{v.Key}").ToList();
                    if (setParts.Count == 0) return 0;
                    var sql = $"UPDATE [{table}] SET {string.Join(", ", setParts)} WHERE [{idField}] = @{idField}";
                    return ExecuteSql(sql, values.ToDictionary(v => $"@{v.Key}", v => v.Value));
                }

                var cols = values.Keys.ToList();
                var sqlInsert = $"INSERT INTO [{table}] ({string.Join(", ", cols.Select(c => $"[{c}]"))}) VALUES ({string.Join(", ", cols.Select(c => $"@{c}"))})";
                return ExecuteSql(sqlInsert, values.ToDictionary(v => $"@{v.Key}", v => v.Value));
            }

            return 0;
        }

        private string ResolveTableName(string procedureName, string operation)
        {
            var candidates = new List<string>();
            if (!string.IsNullOrWhiteSpace(operation))
            {
                candidates.Add(Regex.Replace(operation, "^(rtv|Rtv|Get|Save|Delete|Add|Update)", string.Empty, RegexOptions.IgnoreCase));
            }

            var baseName = procedureName.Replace("sec.", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("S7S", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("SP", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase);
            candidates.Add(baseName);

            var possible = candidates.SelectMany(c => new[] { c, c + "s", "BCM" + c, "S7S" + c, c + "Status", c + "Template" })
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            using var conn = CreateSqliteConnection();
            conn.Open();
            foreach (var p in possible)
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND lower(name)=lower(@name) LIMIT 1";
                cmd.Parameters.AddWithValue("@name", p);
                var found = cmd.ExecuteScalar()?.ToString();
                if (!string.IsNullOrWhiteSpace(found))
                {
                    return found;
                }
            }

            return string.Empty;
        }

        private static string GetParameter(Dictionary<string, object?> parameters, string name)
        {
            if (!parameters.TryGetValue(name, out var value) || value == null)
            {
                return string.Empty;
            }

            return value.ToString() ?? string.Empty;
        }

        private static DataSet EmptyDataSet()
        {
            var ds = new DataSet();
            ds.Tables.Add(new DataTable());
            return ds;
        }
    }
}
