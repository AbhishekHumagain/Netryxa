using Static.API.Configurations;
using Static.API.Middleware.ExceptionHandler;
using Static.API.Serializers;
using Static.Application;
using Static.Infrastructure.Common.LocalizationExtensions;
using Static.Infrastructure.Configurations;
using Static.Infrastructure.Persistence;
using Static.Infrastructure.Persistence.Configurations;
using Static.OpenTelemetry.Enums;
using Static.OpenTelemetry.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json.Serialization;
using RouteDataRequestCultureProvider =
    Static.Infrastructure.Common.LocalizationExtensions.RouteDataRequestCultureProvider;

var builder = WebApplication.CreateBuilder(args);
var isProduction = builder.Environment.IsProduction();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

builder.WebHost.ConfigureEnvironmentVariablesFromDotEnvFile()
       .ConfigureAppConfiguration((_, configurationBuilder) =>
       {
           configurationBuilder.AddEnvironmentVariables();
       });


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddHttpContextAccessor();

builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo> { new("en-US"), new("fr-FR"), new("id-ID") };

    options.DefaultRequestCulture = new(culture: "en-US", uiCulture: "en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders =
    [
        new RouteDataRequestCultureProvider { IndexOfCulture = 1, IndexofUiCulture = 1 }
    ];
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("culture", typeof(LanguageRouteConstraint));
});


builder.Services.AddControllers(opt =>
       {
           opt.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
           // opt.Conventions.Add(new KebabCaseRouteConvention());
       })
       .AddJsonOptions(options =>
       {
           // Serialize/Deserialize enum as string or integer (Backward compatibility)
           // TODO: Use the generic JsonStringEnumConverter<TEnum> for compile-time performance
           options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
           options.JsonSerializerOptions.Converters.Add(new ZuluDateTimeConverter());
           options.JsonSerializerOptions.Converters.Add(new ZuluDateTimeOffsetConverter());
       });

builder.Services.AddSwaggerService();

builder.Services.AddHealthChecks()
       .AddDbContextCheck<StaticDbContext>("database", tags: ["ready"]);

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration, isProduction);

builder.Services.AddWebUiServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCorsServices(builder.Configuration);


builder.Services.AddRouting(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = false;
});

builder.Services.AddExceptionHandler<ValidationExceptionHandler>()
       .AddExceptionHandler<RpcExceptionHandler>()
       .AddExceptionHandler<ExceptionHandler>();

builder.Services.AddStaticOpenTelemetry(
    hostEnvironment: builder.Environment,
    configuration: builder.Configuration,
    instrumentations: InstrumentationType.AspNetCore
                      | InstrumentationType.HttpClient
                      | InstrumentationType.GrpcCore
                      | InstrumentationType.GrpcClient
                      | InstrumentationType.EntityFrameworkCore
                      | InstrumentationType.Redis
                      | InstrumentationType.Npgsql
                      | InstrumentationType.Hangfire
                      | InstrumentationType.Aws);

var app = builder.Build();
app.UseExceptionHandler(_ => { });

app.UseStaticSerilogRequestLogging(builder.Configuration);

app.UseStaticFiles();


app.UseCorsService();

app.UseAuthService();

app.UseSwaggerService(builder.Configuration);

if (!isProduction)
{
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            var result = System.Text.Json.JsonSerializer.Serialize(
                new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString() })
                });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
    });
}
else
{
    app.MapHealthChecks("/health");
}


if (builder.Configuration.GetValue<bool>("HttpsRedirection:Enabled")) app.UseHttpsRedirection();

var localizeOptions = ((IApplicationBuilder)app).ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizeOptions.Value);

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.MapControllers();

app.Run();
public partial class Program;
