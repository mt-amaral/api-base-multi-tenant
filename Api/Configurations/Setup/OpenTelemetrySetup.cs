using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Api.Configurations.Setup;

public static class OpenTelemetrySetup
{
    public static WebApplicationBuilder AddOpenTelemetrySetup(this WebApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            logging.AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://otel-collector:4318"); // OTLP HTTP
                opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("api-base"))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()          // métricas de requisições HTTP
                .AddHttpClientInstrumentation()          // métricas de chamadas HTTP
                .AddRuntimeInstrumentation()             // métricas do .NET (GC, threads)
                .AddPrometheusExporter())
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()          // traces de requisições HTTP
                .AddHttpClientInstrumentation()          // traces de chamadas HTTP
                .AddEntityFrameworkCoreInstrumentation() // traces do EF Core
                .SetSampler(new AlwaysOnSampler())
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri("http://otel-collector:4318"); // OTLP HTTP
                    opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                }));

        return builder;
    }
}
