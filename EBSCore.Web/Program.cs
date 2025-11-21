using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Fillters;
using EBSCore.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;
using System.IO;
using System.Net;
using static System.Reflection.Metadata.BlobBuilder;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Ensure log folder exists early
var logDirectory = Path.Combine(builder.Environment.ContentRootPath, "Logs");
Directory.CreateDirectory(logDirectory);

// Configure Serilog from appsettings.json
builder.Host.UseSerilog((ctx, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .WriteTo.File(Path.Combine(logDirectory, "log-.txt"),
            rollingInterval: RollingInterval.Day,
            shared: true,
            restrictedToMinimumLevel: LogEventLevel.Information);
});

// *** 1. Configure Services ***
builder.Services.AddControllers(); // API controllers
builder.Services.AddServerSideBlazor(); // Blazor Server
builder.Services.AddRazorPages(); // Razor Pages hosting _Host
builder.Services.AddScoped<PageTitleService>();

// *** 2. Register Custom Services ***
builder.Services.AddScoped<LoggingService>(); // Logging service
builder.Services.AddHttpContextAccessor(); // Access to HttpContext

// Configure HttpClient for API calls
builder.Services.AddHttpClient("EBSCoreAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:1490/");
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = new CookieContainer() // Enables cookies
    };
});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("EBSCoreAPI"));
builder.Services.AddHttpClient("WorkflowHttp");
builder.Services.AddHttpClient();

// Register other services
builder.Services.AddSingleton<ServiceLocator>();

builder.Services.AddHostedService<WorkflowBackgroundService>();
builder.Services.AddHostedService<NotificationBackgroundService>();
builder.Services.AddHostedService<S7SNotificationBackgroundService>();

// *** 3. Configure Localization ***
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

//builder.Services.AddHostedService<BGJobs>();

// *** 5. Configure Session ***
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Uncomment if required
});

// *** 6. Build the Application ***
var app = builder.Build();
ServiceLocator.SetServiceProvider(app.Services);

// *** 7. Configure Middleware ***
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Uncomment if HTTPS is required:
    // app.UseHsts();
}

// Uncomment if HTTPS redirection is required:
// app.UseHttpsRedirection();

app.UseStaticFiles(); // Serve static files
app.UseRouting();

app.UseSession(); // Enable session middleware
// Auth guard + Auto-login: runs for every request
app.Use(async (ctx, next) =>
{
    // Allow public and static paths
    var path = ctx.Request.Path;
    bool allow =
        path.StartsWithSegments("/login") ||
        path.StartsWithSegments("/_blazor") ||
        path.StartsWithSegments("/_framework") ||
        path.StartsWithSegments("/assets") ||
        path.StartsWithSegments("/js") ||
        path.StartsWithSegments("/css") ||
        path.StartsWithSegments("/images") ||
        path.StartsWithSegments("/favicon.ico") ||
        path.StartsWithSegments("/api/CurrentUser/Login") ||
        path.StartsWithSegments("/api/workflows/webhook");

    if (!allow)
    {
        var logging = ctx.RequestServices.GetRequiredService<EBSCore.Web.Services.LoggingService>();
        await logging.EnsureLoggedIn();

        // Check again after auto-login
        var hasUser = ctx.Session?.GetString("User");
        if (string.IsNullOrEmpty(hasUser))
        {
            var accept = ctx.Request.Headers["Accept"].ToString();
            if (!string.IsNullOrEmpty(accept) && accept.Contains("text/html", StringComparison.OrdinalIgnoreCase))
            {
                var returnUrl = Uri.EscapeDataString(ctx.Request.Path + ctx.Request.QueryString);
                ctx.Response.Redirect($"/login?returnUrl={returnUrl}");
                return;
            }
            else
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await ctx.Response.WriteAsync("Unauthorized");
                return;
            }
        }
    }

    await next();
});
app.UseAuthorization();

app.MapControllers();

app.MapBlazorHub(); // Enable Blazor Server
app.MapFallbackToPage("/_Host"); // Fallback to Blazor app (_Host.cshtml)

app.Run();
