using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Static.OpenTelemetry.Extensions;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds Serilog request logging middleware to the application pipeline, enriching diagnostic context with HTTP request and response details.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> used to configure the application's request pipeline.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/> instance, used to retrieve the list of blacklisted headers.</param>
    /// <remarks>
    /// This method configures the middleware to:
    /// <list type="bullet">
    /// <item>Log route values from the incoming HTTP request.</item>
    /// <item>Include all query parameters from the request, with support for excluding blacklisted headers.</item>
    /// <item>Capture both request and response headers, omitting headers specified in the "OpenTelemetry:Log:BlackListedHeaders" configuration section.</item>
    /// <item>Utilize Serilog's diagnostic context for enriching log entries with detailed HTTP context information.</item>
    /// </list>
    /// </remarks>

    public static void UseStaticSerilogRequestLogging(this IApplicationBuilder builder, IConfiguration configuration)
    {
        var blackListedHeaders = configuration.GetSection("OpenTelemetry:RedactedHeaders").Get<string[]>() ?? [];

        builder.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("Request_Code_Routes", httpContext.Request.RouteValues);

                foreach (var query in httpContext.Request.Query)
                {
                    diagnosticContext.Set($"Request_Query_{query.Key}", query.Value.ToString());
                }

                foreach (var header in httpContext.Request.Headers.ExceptBy(blackListedHeaders, y => y.Key, StringComparer.OrdinalIgnoreCase))
                {
                    diagnosticContext.Set($"Request_Header_{header.Key}", header.Value.ToString());
                }

                foreach (var header in httpContext.Response.Headers.ExceptBy(blackListedHeaders, y => y.Key, StringComparer.OrdinalIgnoreCase))
                {
                    diagnosticContext.Set($"Response_Header_{header.Key}", header.Value.ToString());
                }
            };
        });
    }
}