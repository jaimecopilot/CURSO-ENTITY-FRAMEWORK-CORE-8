using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace AceriaData.ConsoleApp.Diagnostics;

public static class AzureMonitorOpenTelemetry
{
    public const string ActivitySourceName = "AceriaData";

    public static TracerProvider? CreateFromEnvironment()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "APPLICATIONINSIGHTS_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
            return null;

        return Sdk.CreateTracerProviderBuilder()
            .AddSource(ActivitySourceName)
            .AddAzureMonitorTraceExporter(options =>
                options.ConnectionString = connectionString)
            .Build();
    }
}
