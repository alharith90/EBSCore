using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EBSCore.Web.WorkflowEngine.Application.Execution;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Entities;
using EBSCore.Web.WorkflowEngine.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EBSCore.Web.WorkflowEngine.Application.Execution.Strategies;

public class HttpNodeExecutor : INodeExecutorStrategy
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICredentialRepository _credentialRepository;
    private readonly ILogger<HttpNodeExecutor> _logger;

    public HttpNodeExecutor(IHttpClientFactory httpClientFactory, ICredentialRepository credentialRepository, ILogger<HttpNodeExecutor> logger)
    {
        _httpClientFactory = httpClientFactory;
        _credentialRepository = credentialRepository;
        _logger = logger;
    }

    public bool CanExecute(string nodeType) => string.Equals(nodeType, NodeType.Http, StringComparison.OrdinalIgnoreCase);

    public async Task<NodeExecutionResult> ExecuteAsync(NodeExecutionContext context, CancellationToken cancellationToken)
    {
        var config = JsonSerializer.Deserialize<HttpNodeConfig>(context.Node.ConfigJson ?? "{}") ?? new HttpNodeConfig();
        if (string.IsNullOrWhiteSpace(config.Url))
        {
            return new NodeExecutionResult { Success = false, ErrorMessage = "HTTP node requires a Url value." };
        }

        var method = new HttpMethod(string.IsNullOrWhiteSpace(config.Method) ? "GET" : config.Method!.ToUpperInvariant());
        var url = TemplateEvaluator.Render(config.Url, context);
        var request = new HttpRequestMessage(method, url);

        if (!string.IsNullOrWhiteSpace(config.Body))
        {
            var content = TemplateEvaluator.Render(config.Body, context);
            request.Content = new StringContent(content, Encoding.UTF8, config.ContentType ?? "application/json");
        }

        if (config.Headers != null)
        {
            foreach (var header in config.Headers)
            {
                var value = TemplateEvaluator.Render(header.Value, context);
                if (!request.Headers.TryAddWithoutValidation(header.Key, value))
                {
                    request.Content ??= new StringContent(string.Empty);
                    request.Content.Headers.TryAddWithoutValidation(header.Key, value);
                }
            }
        }

        if (context.Node.CredentialId.HasValue)
        {
            var credential = await _credentialRepository.GetCredentialAsync(context.Node.CredentialId.Value, cancellationToken);
            if (credential != null)
            {
                ApplyCredential(request, credential);
            }
        }

        var client = _httpClientFactory.CreateClient("WorkflowHttp");
        var response = await client.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        var outputs = new Dictionary<string, object?>
        {
            ["statusCode"] = (int)response.StatusCode,
            ["body"] = body,
            ["headers"] = response.Headers.ToDictionary(h => h.Key, h => string.Join(",", h.Value))
        };

        if (config.ParseJson)
        {
            try
            {
                var json = JsonDocument.Parse(body);
                outputs["json"] = json.RootElement.Clone();
                outputs["result"] = json.RootElement.Clone();
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Unable to parse HTTP response as JSON for node {NodeId}.", context.Node.NodeId);
                outputs["result"] = body;
            }
        }
        else
        {
            outputs["result"] = body;
        }

        return new NodeExecutionResult
        {
            Success = response.IsSuccessStatusCode,
            ErrorMessage = response.IsSuccessStatusCode ? null : body,
            Outputs = outputs
        };
    }

    private static void ApplyCredential(HttpRequestMessage request, Credential credential)
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(credential.DataJson ?? "{}") ?? new Dictionary<string, string>();
        switch (credential.CredentialType)
        {
            case "HTTPBasic":
                if (data.TryGetValue("username", out var username) && data.TryGetValue("password", out var password))
                {
                    var value = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", value);
                }
                break;
            case "BearerToken":
                if (data.TryGetValue("token", out var token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                break;
            case "ApiKey":
                if (data.TryGetValue("headerName", out var headerName) && data.TryGetValue("value", out var keyValue))
                {
                    request.Headers.TryAddWithoutValidation(headerName, keyValue);
                }
                break;
        }
    }

    private sealed class HttpNodeConfig
    {
        public string? Url { get; set; }
        public string? Method { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? Body { get; set; }
        public string? ContentType { get; set; }
        public bool ParseJson { get; set; } = true;
    }
}
