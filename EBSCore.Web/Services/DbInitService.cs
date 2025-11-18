using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
            var applyEmployeeSP = _config.GetValue<bool>("DbInit:ApplyEmployeeSP");
            if (!applyEmployeeSP) return;

            try
            {
                var cs = _config.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(cs))
                {
                    _logger.LogWarning("DefaultConnection is empty; skipping DbInit.");
                    return;
                }

                var scriptPath = Path.Combine(_env.ContentRootPath, "AppData", "EmployeeSP.sql");
                if (!File.Exists(scriptPath))
                {
                    _logger.LogWarning("EmployeeSP.sql not found at {Path}; skipping.", scriptPath);
                    return;
                }

                var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
                var batches = Regex.Split(script, @"^\s*GO\s*;?\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                using var conn = new SqlConnection(cs);
                await conn.OpenAsync(cancellationToken);
                foreach (var batch in batches)
                {
                    var sql = batch.Trim();
                    if (string.IsNullOrWhiteSpace(sql)) continue;
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 120;
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                _logger.LogInformation("EmployeeSP applied successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying EmployeeSP.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
