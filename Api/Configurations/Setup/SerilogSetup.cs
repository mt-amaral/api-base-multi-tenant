using Serilog;

namespace Api.Configurations.Setup;

public static class SerilogSetup
{
    public static WebApplicationBuilder AddSerilogSetup(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration)
                  .Enrich.WithProperty("Application", "api-base")
                  .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

            if (context.HostingEnvironment.IsProduction())
            {
                config.WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = "http://otel-collector:4318";
                    options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.HttpProtobuf;
                    options.ResourceAttributes.Add("service.name", "api-base");
                });
            }
            else
            {
                config.WriteTo.Console();
            }
        });

        return builder;
    }
}
