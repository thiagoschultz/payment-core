using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Payment.Api.Middleware;
using Payment.Infrastructure.Cache;
using Payment.Infrastructure.Data;
using payment_core.src.Payment.Api;
using Serilog;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//
// SERILOG
//

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();


//
// CONTROLLERS
//

builder.Services.AddControllers();


//
// SWAGGER
//

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Payment Core API",
        Version = "v1"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});



//
// SQL SERVER
//

builder.Services.AddDbContext<PaymentDbContext>(options =>
{
    options.UseSqlServer(
        configuration.GetConnectionString("sql"));
});



//
// REDIS
//

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(
        configuration.GetConnectionString("redis"));
});

builder.Services.AddScoped<RedisService>();



//
// RATE LIMIT
//

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(
    configuration.GetSection("IpRateLimiting"));

builder.Services.AddInMemoryRateLimiting();

builder.Services.AddSingleton<IRateLimitConfiguration,
RateLimitConfiguration>();



//
// HEALTH CHECKS
//

builder.Services.AddHealthChecks()
    .AddSqlServer(configuration.GetConnectionString("sql"))
    .AddRedis(configuration.GetConnectionString("redis"));



//
// OPENTELEMETRY (TRACE + METRICS)
//

builder.Services.AddOpenTelemetry()

    .ConfigureResource(resource =>
    {
        resource.AddService(
            serviceName: "payment-api",
            serviceVersion: "1.0");
    })

    .WithTracing(tracing =>
    {

        tracing.AddAspNetCoreInstrumentation();

        tracing.AddHttpClientInstrumentation();

        tracing.AddEntityFrameworkCoreInstrumentation();

    })

    .WithMetrics(metrics =>
    {

        metrics.AddAspNetCoreInstrumentation();

        metrics.AddHttpClientInstrumentation();

        metrics.AddRuntimeInstrumentation();

        metrics.AddPrometheusExporter();

    });



var app = builder.Build();



//
// SWAGGER
//

app.UseSwagger();

app.UseSwaggerUI();



//
// RATE LIMIT
//

app.UseIpRateLimiting();



//
// TENANT
//

app.UseMiddleware<TenantMiddleware>();



//
// ROUTING
//

app.UseRouting();



//
// AUTH READY
//

app.UseAuthorization();



//
// PROMETHEUS ENDPOINT
//

app.MapPrometheusScrapingEndpoint();



//
// HEALTH
//

app.MapHealthChecks("/health");



//
// CONTROLLERS
//

app.MapControllers();



app.Run();