using Static.Helper.Constants;

namespace Static.API.Configurations;

/// <summary>
/// The Cors Configuration
/// </summary>
public static class CorsConfiguration
{
    private static string AllowedOrigins => "AllowedOrigins";
    /// <summary>
    /// This Service Contain Logic for Cors
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        StaticConstants.WhitelistedDomains = configuration.GetSection("App:CorsOrigins").Get<string[]>();
        StaticConstants.StaticTermsAndConditionsUrl =  configuration.GetSection("App:EndUserAppUrl").Get<string>()+"/terms-and-condition";
        StaticConstants.StaticPrivacyPolicyUrl = configuration.GetSection("App:EndUserAppUrl").Get<string>()+"/privacy-policy";
        
       services.AddCors(options =>
        {
            options.AddPolicy(AllowedOrigins,
                policy =>
                {
                    policy.WithOrigins(StaticConstants.WhitelistedDomains!)
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

    }
    
    /// <summary>
    /// The Service Use Cors
    /// </summary>
    /// <param name="app"></param>
    public static void UseCorsService(this WebApplication app)
    {
        app.UseCors(AllowedOrigins);
    }
}