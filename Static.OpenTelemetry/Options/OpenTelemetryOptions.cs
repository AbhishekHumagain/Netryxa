namespace Static.OpenTelemetry.Options;

internal class OpenTelemetryOptions
{
    internal const string SectionName = "OpenTelemetry";
    internal required string CollectorEndpoint { get; init; }
    internal string[] RedactedHeaders { get; init; } = [];
    internal required TraceConfig Trace { get; init; }
    internal required MetricConfig Metric { get; init; }
    internal required LogConfig Log { get; init; }
}

internal class TraceConfig
{
    internal bool ExportToConsole { get; init; }
}

internal class MetricConfig
{
    internal string[] Meters { get; init; } = [];
    internal bool ExportToConsole { get; init; }
}

internal class LogConfig
{
    internal bool ExportToConsole { get; init; }
}