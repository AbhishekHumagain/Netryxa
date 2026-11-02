namespace Static.OpenTelemetry.Enums;

/// <summary>
/// Defines the available instrumentation types that can be enabled for OpenTelemetry.
/// Each value corresponds to a different type of instrumentation that can be used to collect telemetry data.
/// </summary>
[Flags]
public enum InstrumentationType
{
    AspNetCore = 1,
    HttpClient = 2,
    Redis = 4,
    EntityFrameworkCore = 8,
    GrpcClient = 16,
    GrpcCore = 32,
    Aws = 64,
    Hangfire = 128,
    Npgsql = 256
}