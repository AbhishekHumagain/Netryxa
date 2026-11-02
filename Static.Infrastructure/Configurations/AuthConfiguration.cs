using Static.Cryptography;
using Static.Cryptography.CipherConfig;
using Static.Helper.Constants;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Static.Infrastructure.Configurations;

public static class AuthConfiguration
{
    public static void AddAuthService(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        var jwtSettings = configuration.GetSection("Authentication:JwtBearer");
        var securityKey = jwtSettings["SecurityKey"];
        ArgumentNullException.ThrowIfNull(securityKey);
        var audience = jwtSettings["Audience"];
        var issuer = jwtSettings["Issuer"];

        var tokenPassPhrase = configuration["Encryption:TokenPassPhrase"];
        ArgumentNullException.ThrowIfNull(tokenPassPhrase);

        services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey)),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                    options.Events = new()
                    {
                        OnMessageReceived = async context =>
                        {
                            var request = context.Request;
                            var headerDevice = request.Headers[StaticConstants.HttpHeaders.Device].ToString();
                            var accessToken = headerDevice switch
                            {
                                StaticConstants.XDevice.MstMobile or StaticConstants.XDevice.MstTv =>
                                    request.Headers.Authorization.ToString(),

                                StaticConstants.XDevice.MstEndUser or StaticConstants.XDevice.MstBackOffice =>
                                    request.Cookies[StaticConstants.Cookies.AccessToken],

                                _ when !isProduction =>
                                    request.Headers.Authorization == string.Empty
                                        // For testing: Bearer token prioritization, Cookie as fallback.
                                        ? request.Headers.Authorization.ToString()
                                        : request.Cookies[StaticConstants.Cookies.AccessToken],

                                _ => null
                            };

                            if (string.IsNullOrEmpty(accessToken))
                            {
                                var hasAuthorizeAttribute = context.HttpContext
                                                                   .GetEndpoint()?
                                                                   .Metadata
                                                                   .Count(x => x is AuthorizeAttribute) > 0;

                                if (!hasAuthorizeAttribute) return;

                                context.HttpContext
                                       .RequestServices
                                       .GetRequiredService<ILogger<MessageReceivedContext>>()
                                       .LogWarning(
                                           "Missing access token for '{Platform}' platform. Environment: {Environment}.",
                                           headerDevice,
                                           isProduction ? "Production" : "Non-Production");
                                return;
                            }

                            context.Token = await StaticCipher
                                                  .GetCipher(new AesCipherConfig { PassPhrase = tokenPassPhrase })
                                                  .Decrypt(accessToken);
                        }
                    };
                });

        services.AddAuthorization();
    }

    public static void UseAuthService(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}