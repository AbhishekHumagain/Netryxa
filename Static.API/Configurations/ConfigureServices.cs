using Static.Infrastructure.Common;
using Static.Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc;
using Static.API.Grpc.Interceptors;
using Static.API.Middleware.ExceptionHandler;
using Static.Helper.Constants;
using System.Net;
using Static.Results;

namespace Static.API.Configurations;

/// <summary>
/// The Configure Service
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// The Web Ui Service
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebUiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // This will get and set the value from configurations.
        configuration.GetValues<TvConfigurationGetValue>("TV");
        configuration.GetValues<WssConfigurationsGetValue>("WebSocket");
        StaticConstants.IvyWebhookSecret = configuration.GetSection("GRPC:IvyToken").Value!;
        StaticConstants.TestpaperToken = configuration.GetSection("Testpaper:CallbackToken").Value!;
        StaticConstants.NotificationToken = configuration.GetSection("GRPC:NotificationToken").Value!;

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                Result result = ErrorResponseBuilder.CreateBuilder(HttpStatusCode.BadRequest,
                                                        "Invalid request payload. Please check the input data.")
                                                    .AddType("InvalidModelState")
                                                    .AddDetails(context.ModelState
                                                                     .Where(x => x.Value is { Errors.Count: > 0 })
                                                                     .ToDictionary(
                                                                         x => x.Key,
                                                                         y => y.Value?.Errors
                                                                               .Select(x => x.ErrorMessage).ToArray()))
                                                    .BuildError();

                return new BadRequestObjectResult(result);
            };
        });

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ErrorInterceptor>();
        });

        return services;
    }
}