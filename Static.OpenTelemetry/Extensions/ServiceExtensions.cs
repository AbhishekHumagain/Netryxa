using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using Serilog.Sinks.SystemConsole.Themes;
using Static.OpenTelemetry.Enums;
using Static.OpenTelemetry.Options;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ExportProcessorType = OpenTelemetry.ExportProcessorType;
using SuppressInstrumentationScope = OpenTelemetry.SuppressInstrumentationScope;

namespace Static.OpenTelemetry.Extensions;

public static partial class ServiceExtensions
{
    /// <summary>
    /// Configures OpenTelemetry and Serilog for the application, enabling tracing, metrics collection, and logging.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the OpenTelemetry and Serilog services will be added.</param>
    /// <param name="hostEnvironment">The current <see cref="IHostEnvironment"/> providing information about the hosting environment (e.g., development, production).</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance, used to load configuration settings for OpenTelemetry, including the collector endpoint and other options.</param>
    /// <param name="instrumentations">The <see cref="InstrumentationType"/> flag to specify which instrumentations should be enabled (e.g., ASP.NET Core, HttpClient, Redis, etc.).</param>
    /// <exception cref="NullReferenceException">Thrown if <see cref="OpenTelemetryOptions"/> is not properly configured in the application settings.</exception>
    /// <remarks>
    /// This method performs the following tasks:
    /// <list type="bullet">
    /// <item>Loads and validates the <see cref="OpenTelemetryOptions"/> from the configuration.</item>
    /// <item>Collects metadata about the assembly, environment, runtime, host, and operating system as static attributes for tracing and metrics.</item>
    /// <item>Configures OpenTelemetry for both tracing and metrics:
    /// <list type="bullet">
    /// <item>Enables various instrumentations based on the provided <paramref name="instrumentations"/> flag (e.g., ASP.NET Core, HttpClient, Redis, Entity Framework Core, etc.).</item>
    /// <item>Sets up OTLP exporters for both trace and metric data, with an optional console exporter for debugging purposes.</item>
    /// </list>
    /// </item>
    /// <item>Integrates Serilog logging with OpenTelemetry:
    /// <list type="bullet">
    /// <item>Enriches logs with process, thread, and memory usage information.</item>
    /// <item>Configures OpenTelemetry exporter for logs, if a valid collector endpoint is provided.</item>
    /// <item>Optionally outputs logs to the console and/or debug output, based on configuration.</item>
    /// </list>
    /// </item>
    /// </list>
    /// </remarks>
    public static void AddStaticOpenTelemetry(this IServiceCollection services, IHostEnvironment hostEnvironment, IConfiguration configuration, InstrumentationType instrumentations)
    {
        #region OpenTelemetry options configuration

        var openTelemetryOptions = configuration
                                   .GetSection(OpenTelemetryOptions.SectionName)
                                   .Get<OpenTelemetryOptions>(options => { options.BindNonPublicProperties = true; })
                                   ?? throw new NullReferenceException($"{nameof(OpenTelemetryOptions)} is not set in configuration!");

        #endregion

        #region Info

        #region Calling Assembly Info

        var callingAssembly = Assembly.GetCallingAssembly();
        var callingAssemblyName = callingAssembly.GetName();

        #endregion

        #region Service Info

        var serviceName = callingAssemblyName.Name ?? "unknown_service";
        var serviceNameSpace = callingAssemblyName.Name == null
            ? "unknown_service_namespace"
            : SplitBySymbolsRegex().Split(callingAssemblyName.Name)[0];
        var serviceVersion = callingAssemblyName.Version?.ToString() ?? "unknown_version";
        var serviceInformationalVersion = callingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown_informational_version";

        #endregion

        #region Environment Info

        var environmentName = hostEnvironment.EnvironmentName;
        var environmentUserName = ((!string.IsNullOrWhiteSpace(Environment.UserDomainName))
            ? (Environment.UserDomainName + "\\" + Environment.UserName)
            : Environment.UserName);

        #endregion

        #region Runtime Info

        const string runtimeName = ".NET";
        var runtimeVersion = Environment.Version.ToString();
        var runtimeDescription = RuntimeInformation.FrameworkDescription;

        #endregion

        #region Host Info

        var hostName = Environment.MachineName;
        var hostArchitecture = RuntimeInformation.ProcessArchitecture.ToString();

        #endregion

        #region OS Info

        var osType = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "windows"
            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? "linux"
                : "unknown";

        var osArchitecture = RuntimeInformation.OSArchitecture.ToString();
        var osVersion = Environment.OSVersion.ToString();
        var osDescription = RuntimeInformation.OSDescription;
        var osRuntimeId = RuntimeInformation.RuntimeIdentifier;

        #endregion

        #region Static Attributes

        var attributes = new Dictionary<string, object>
        {
            ["service.informational_version"] = serviceInformationalVersion,
            ["deployment.environment"] = environmentName,
            ["deployment.environment_username"] = environmentUserName,
            ["host.name"] = hostName,
            ["host.arch"] = hostArchitecture,
            ["os.type"] = osType,
            ["os.arch"] = osArchitecture,
            ["os.version"] = osVersion,
            ["os.description"] = osDescription,
            ["os.runtime_id"] = osRuntimeId,
            ["runtime.name"] = runtimeName,
            ["runtime.version"] = runtimeVersion,
            ["runtime.description"] = runtimeDescription,
        };

        #endregion

        #endregion

        #region Add OpenTelemetry for Tracing and Metrics

        if (!string.IsNullOrEmpty(openTelemetryOptions.CollectorEndpoint))
        {
            var openTelemetryBuilderBuilder = services.AddOpenTelemetry()
                                                      .ConfigureResource(resourceBuilder => resourceBuilder.AddService
                                                                                                           (
                                                                                                               serviceName: serviceName,
                                                                                                               serviceNamespace: serviceNameSpace,
                                                                                                               serviceVersion: serviceVersion
                                                                                                           )
                                                                                                           .AddAttributes(attributes));

            #region Add Tracing

            openTelemetryBuilderBuilder.WithTracing(builder =>
            {
                builder.AddSource("Static.MediatR");
                if (instrumentations.HasFlag(InstrumentationType.AspNetCore))
                {
                    builder.AddAspNetCoreInstrumentation(x =>
                    {
                        x.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.DisplayName = $"{httpRequest.Method} {httpRequest.Path}";
                        };

                        x.Filter = httpContext =>
                        {
                            if (httpContext.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase))
                            {
                                return false;
                            }

                            return true;
                        };
                    });
                }

                if (instrumentations.HasFlag(InstrumentationType.HttpClient))
                {
                    builder.AddHttpClientInstrumentation();
                }

                if (instrumentations.HasFlag(InstrumentationType.Redis))
                {
                    builder.AddRedisInstrumentation(options => options.SetVerboseDatabaseStatements = true);
                }

                if (instrumentations.HasFlag(InstrumentationType.EntityFrameworkCore))
                {
                    builder.AddEntityFrameworkCoreInstrumentation();
                }

                if (instrumentations.HasFlag(InstrumentationType.GrpcClient))
                {
                    builder.AddGrpcClientInstrumentation();
                }

                if (instrumentations.HasFlag(InstrumentationType.GrpcCore))
                {
                    builder.AddGrpcCoreInstrumentation();
                }

                if (instrumentations.HasFlag(InstrumentationType.Aws))
                {
                    builder.AddAWSInstrumentation();
                }

                if (instrumentations.HasFlag(InstrumentationType.Hangfire))
                {
                    builder.AddHangfireInstrumentation();
                }

                if (instrumentations.HasFlag(InstrumentationType.Npgsql))
                {
                    builder.AddNpgsql();
                }

                builder.AddOtlpExporter(options =>
                {
                    options.ExportProcessorType = ExportProcessorType.Batch;
                    options.Protocol = OtlpExportProtocol.Grpc;
                    options.Endpoint = new(openTelemetryOptions.CollectorEndpoint);
                });

                if (openTelemetryOptions.Trace.ExportToConsole)
                {
                    builder.AddConsoleExporter();
                }
            });

            #endregion

            #region Add Metrics

            openTelemetryBuilderBuilder.WithMetrics(builder =>
            {
                builder
                    .AddMeter(openTelemetryOptions.Metric.Meters)
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation();

                if (instrumentations.HasFlag(InstrumentationType.AspNetCore))
                {
                    builder.AddAspNetCoreInstrumentation();
                }

                if (instrumentations.HasFlag(InstrumentationType.HttpClient))
                {
                    builder.AddHttpClientInstrumentation();
                }

                if (instrumentations.HasFlag(InstrumentationType.Aws))
                {
                    builder.AddAWSInstrumentation();
                }

                builder.AddOtlpExporter(options =>
                {
                    options.ExportProcessorType = ExportProcessorType.Batch;
                    options.Protocol = OtlpExportProtocol.Grpc;
                    options.Endpoint = new(openTelemetryOptions.CollectorEndpoint);
                });

                if (openTelemetryOptions.Metric.ExportToConsole)
                {
                    builder.AddConsoleExporter();
                }

                builder.SetExemplarFilter(ExemplarFilterType.TraceBased);
            });
        }

        #endregion

        #endregion

        #region Add Serilog for Logging with OpenTelemetry Exporter

        var loggerConfiguration = new LoggerConfiguration()
                                  .Enrich.WithProcessId()
                                  .Enrich.WithProcessName()
                                  .Enrich.WithThreadId()
                                  .Enrich.WithThreadName()
                                  .Enrich.WithMemoryUsage()
                                  .Enrich.FromLogContext();

        if (!string.IsNullOrEmpty(openTelemetryOptions.CollectorEndpoint))
        {
            loggerConfiguration
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = openTelemetryOptions.CollectorEndpoint;
                    options.Protocol = OtlpProtocol.Grpc;
                    options.OnBeginSuppressInstrumentation = SuppressInstrumentationScope.Begin;
                    options.ResourceAttributes = new Dictionary<string, object>(attributes) { ["service.name"] = serviceName, ["service.namespace"] = serviceNameSpace, ["service.version"] = serviceVersion, ["service.informational_version"] = serviceInformationalVersion };
                });
        }

        if (openTelemetryOptions.Log.ExportToConsole)
        {
            loggerConfiguration
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.Debug();
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        services.AddSerilog();

        #endregion
    }

    [GeneratedRegex(@"[\W_]+")]
    private static partial Regex SplitBySymbolsRegex();
}