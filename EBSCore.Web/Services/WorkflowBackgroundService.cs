using EBSCore.AdoClass;
using EBSCore.Web.Models.Workflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EBSCore.Web.Services
{
    public class WorkflowBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WorkflowBackgroundService> _logger;

        public WorkflowBackgroundService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory, ILogger<WorkflowBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var executionSP = new DBWorkflowExecutionSP(configuration);
                    _logger.LogInformation("[Workflow] Polling for executions");

                    var dataSet = (DataSet)executionSP.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset, Operation: "DequeueExecution");

                    if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    {
                        _logger.LogDebug("[Workflow] No executions dequeued");
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                        continue;
                    }

                    await ProcessExecutionAsync(executionSP, dataSet, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Workflow background service error");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private async Task ProcessExecutionAsync(DBWorkflowExecutionSP executionSP, DataSet dataSet, CancellationToken stoppingToken)
        {
            var executionRow = dataSet.Tables[0].Rows[0];
            var executionId = Convert.ToInt64(executionRow["ExecutionID"]);

            try
            {
                _logger.LogInformation("[Workflow] Starting execution {ExecutionId} with trigger {TriggerType}", executionId, executionRow["TriggerType"]);

                var nodes = new List<WorkflowNodeModel>();
                if (dataSet.Tables.Count > 1)
                {
                    foreach (DataRow row in dataSet.Tables[1].Rows)
                    {
                        nodes.Add(new WorkflowNodeModel
                        {
                            NodeID = Convert.ToInt32(row["NodeID"]),
                            NodeKey = $"node-{row["NodeID"]}",
                            Name = row["Name"].ToString(),
                            NodeType = row["NodeType"].ToString(),
                            ConfigJson = row["ConfigJson"].ToString(),
                            CredentialID = row["CredentialID"] == DBNull.Value ? null : Convert.ToInt32(row["CredentialID"]),
                            RetryCount = row["RetryCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["RetryCount"])
                        });
                    }
                }

                var connections = new List<WorkflowConnectionModel>();
                if (dataSet.Tables.Count > 2)
                {
                    foreach (DataRow row in dataSet.Tables[2].Rows)
                    {
                        connections.Add(new WorkflowConnectionModel
                        {
                            SourceNodeID = Convert.ToInt32(row["SourceNodeID"]),
                            TargetNodeID = Convert.ToInt32(row["TargetNodeID"]),
                            SourceOutputKey = row["SourceOutputKey"].ToString(),
                            TargetInputKey = row["TargetInputKey"].ToString()
                        });
                    }
                }

                _logger.LogInformation("[Workflow] Loaded {NodeCount} nodes and {ConnectionCount} connections for execution {ExecutionId}", nodes.Count, connections.Count, executionId);

                var nodeMap = nodes.Where(n => n.NodeID.HasValue).ToDictionary(n => n.NodeID!.Value, n => n);
                var pending = new HashSet<int>(nodeMap.Keys);
                var results = new Dictionary<int, NodeExecutionResult>();
                var client = _httpClientFactory.CreateClient("WorkflowHttp");
                bool progress;

                do
                {
                    progress = false;
                    foreach (var nodeId in pending.ToList())
                    {
                        if (!CanExecuteNode(nodeId, connections, results))
                        {
                            continue;
                        }

                        var node = nodeMap[nodeId];
                        _logger.LogInformation("[Workflow] Executing node {NodeId} ({NodeName}) of type {NodeType}", nodeId, node.Name, node.NodeType);

                        try
                        {
                            var result = await ExecuteNodeAsync(node, executionRow, client, stoppingToken);
                            results[nodeId] = result;
                            await SaveStepAsync(executionSP, executionId, nodeId, "Succeeded", result.OutputKey, result.OutputJson);
                            _logger.LogInformation("[Workflow] Node {NodeId} finished with key {OutputKey}", nodeId, result.OutputKey);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[Workflow] Node {NodeId} failed", nodeId);
                            await SaveStepAsync(executionSP, executionId, nodeId, "Failed", null, ex.Message);
                            executionSP.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                                Operation: "MarkExecutionStatus",
                                ExecutionID: executionId.ToString(),
                                Status: "Failed",
                                ErrorMessage: ex.Message);
                            return;
                        }

                        pending.Remove(nodeId);
                        progress = true;
                    }
                }
                while (pending.Count > 0 && progress);

                if (pending.Count > 0)
                {
                    var message = "Workflow halted due to missing dependencies";
                    _logger.LogWarning("[Workflow] Execution {ExecutionId} halted: {Message}", executionId, message);
                    executionSP.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                        Operation: "MarkExecutionStatus",
                        ExecutionID: executionId.ToString(),
                        Status: "Failed",
                        ErrorMessage: message);
                }
                else
                {
                    _logger.LogInformation("[Workflow] Execution {ExecutionId} succeeded", executionId);
                    executionSP.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                        Operation: "MarkExecutionStatus",
                        ExecutionID: executionId.ToString(),
                        Status: "Succeeded");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Workflow] Fatal error while processing execution {ExecutionId}", executionId);
                executionSP.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                    Operation: "MarkExecutionStatus",
                    ExecutionID: executionId.ToString(),
                    Status: "Failed",
                    ErrorMessage: ex.Message);
            }
        }

        private bool CanExecuteNode(int nodeId, List<WorkflowConnectionModel> connections, Dictionary<int, NodeExecutionResult> results)
        {
            var inbound = connections.Where(c => c.TargetNodeID == nodeId).ToList();
            if (!inbound.Any())
            {
                return true;
            }

            foreach (var connection in inbound)
            {
                if (connection.SourceNodeID == null)
                {
                    continue;
                }

                if (!results.TryGetValue(connection.SourceNodeID.Value, out var output))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(connection.SourceOutputKey) || string.Equals(connection.SourceOutputKey, output.OutputKey, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<NodeExecutionResult> ExecuteNodeAsync(WorkflowNodeModel node, DataRow executionRow, HttpClient client, CancellationToken stoppingToken)
        {
            var type = node.NodeType?.ToLowerInvariant();
            var configJson = node.ConfigJson ?? "{}";
            using var document = JsonDocument.Parse(configJson);
            var config = document.RootElement.Clone();

            try
            {
                return type switch
                {
                    "delay" => await RunDelayNodeAsync(config, stoppingToken),
                    "http" or "httprequest" => await RunHttpNodeAsync(config, client, stoppingToken),
                    "code" => RunCodeNode(config),
                    "conditional" => RunConditionalNode(config),
                    _ => RunPassThroughNode(config, executionRow)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Workflow] Error executing node {NodeId} of type {NodeType}", node.NodeID, node.NodeType);
                throw;
            }
        }

        private async Task<NodeExecutionResult> RunDelayNodeAsync(JsonElement config, CancellationToken token)
        {
            var seconds = config.TryGetProperty("seconds", out var value) && value.ValueKind == JsonValueKind.Number
                ? value.GetInt32()
                : 1;
            await Task.Delay(TimeSpan.FromSeconds(Math.Max(0, seconds)), token);
            var payload = new { delay = seconds };
            return new NodeExecutionResult("delay", JsonConvert.SerializeObject(payload));
        }

        private async Task<NodeExecutionResult> RunHttpNodeAsync(JsonElement config, HttpClient client, CancellationToken token)
        {
            var method = config.TryGetProperty("method", out var methodJson) ? methodJson.GetString() : "GET";
            var url = config.TryGetProperty("url", out var urlJson) ? urlJson.GetString() : null;
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("HTTP node missing URL");
            }

            var request = new HttpRequestMessage(new HttpMethod(method ?? "GET"), url);
            _logger.LogInformation("[Workflow] HTTP node request {Method} {Url}", request.Method, request.RequestUri);
            if (config.TryGetProperty("headers", out var headers) && headers.ValueKind == JsonValueKind.Object)
            {
                foreach (var header in headers.EnumerateObject())
                {
                    request.Headers.TryAddWithoutValidation(header.Name, header.Value.GetString());
                }
            }

            if (config.TryGetProperty("body", out var body) && body.ValueKind != JsonValueKind.Null)
            {
                var contentType = config.TryGetProperty("contentType", out var ct) ? ct.GetString() ?? "application/json" : "application/json";
                var payload = body.ValueKind == JsonValueKind.String ? body.GetString() : body.GetRawText();
                request.Content = new StringContent(payload ?? string.Empty, Encoding.UTF8, contentType);
            }

            var response = await client.SendAsync(request, token);
            var responseBody = await response.Content.ReadAsStringAsync(token);
            _logger.LogInformation("[Workflow] HTTP node response {StatusCode} {Url}", (int)response.StatusCode, request.RequestUri);
            var result = new { statusCode = (int)response.StatusCode, body = responseBody };
            var key = response.IsSuccessStatusCode ? "success" : "error";
            return new NodeExecutionResult(key, JsonConvert.SerializeObject(result));
        }

        private NodeExecutionResult RunCodeNode(JsonElement config)
        {
            var resultValue = config.TryGetProperty("returnValue", out var returnValue) ? returnValue.GetString() : config.ToString();
            return new NodeExecutionResult("code", JsonConvert.SerializeObject(new { value = resultValue }));
        }

        private NodeExecutionResult RunConditionalNode(JsonElement config)
        {
            var selectedKey = config.TryGetProperty("trueKey", out var trueKey) ? trueKey.GetString() : "true";
            var fallbackKey = config.TryGetProperty("falseKey", out var falseKey) ? falseKey.GetString() : "false";
            var resultValue = config.TryGetProperty("result", out var resultJson) && resultJson.ValueKind == JsonValueKind.True;
            return new NodeExecutionResult(resultValue ? selectedKey : fallbackKey, JsonConvert.SerializeObject(new { condition = resultValue }));
        }

        private NodeExecutionResult RunPassThroughNode(JsonElement config, DataRow executionRow)
        {
            var payload = new
            {
                executionId = executionRow["ExecutionID"],
                triggerType = executionRow["TriggerType"],
                triggerData = executionRow["TriggerDataJson"],
                webhookData = executionRow["WebhookRequestJson"],
                config = config.GetRawText()
            };
            return new NodeExecutionResult(null, JsonConvert.SerializeObject(payload));
        }

        private Task SaveStepAsync(DBWorkflowExecutionSP sp, long executionId, int nodeId, string status, string? outputKey, string? payload)
        {
            try
            {
                sp.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveExecutionStep",
                    ExecutionID: executionId.ToString(),
                    NodeID: nodeId.ToString(),
                    Status: status,
                    OutputKey: outputKey,
                    OutputJson: payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Workflow] Failed to save execution step for execution {ExecutionId} node {NodeId}", executionId, nodeId);
            }

            return Task.CompletedTask;
        }

        private record NodeExecutionResult(string? OutputKey, string? OutputJson);
    }
}
