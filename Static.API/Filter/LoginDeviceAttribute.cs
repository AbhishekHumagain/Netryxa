using Static.Helper.Constants;
using Static.Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace Static.API.Filter;

/// <summary>
/// This will validate the login device 
/// </summary>
public class LoginDeviceAttribute : Attribute, IAsyncAuthorizationFilter
{
    /// <summary>
    /// Implementing OnAuthorizations
    /// </summary>
    /// <param name="context"></param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var headerDevice = string.Empty;
        var headerContent = context.HttpContext!.Request.Headers.ContainsKey(StaticConstants.HttpHeaders.Device);
        if (headerContent)
        {
            headerDevice = context.HttpContext!.Request.Headers[StaticConstants.HttpHeaders.Device];

            if (headerDevice is StaticConstants.XDevice.MstTv)
            {
                try
                {
                    var token = GetTokenFromHeader(context);
            
                    if (string.IsNullOrEmpty(token))
                    {
                        SendErrorResponse(context, "Authorization header is missing or invalid");
                        return;
                    }

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = GetTokenValidationParameters(context);
            
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
                    var jwtToken = (JwtSecurityToken)validatedToken;
                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        SendErrorResponse(context, "Token has expired");
                        return;
                    }
            
                    context.HttpContext.User = principal;
                }
                catch (SecurityTokenException ex)
                {
                    SendErrorResponse(context, $"Token validation failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    SendErrorResponse(context, "An error occurred during authorization");
                }
            }
        }
        return;
    }
    
    private static void SendErrorResponse(AuthorizationFilterContext context, string message)
    {
        context.Result = new ContentResult()
        {
            StatusCode = (int) HttpStatusCode.Unauthorized,
            Content = message
        };
    }
    
    private static string GetTokenFromHeader(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader) && 
            authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return null;
    }
    
    private static TokenValidationParameters GetTokenValidationParameters(AuthorizationFilterContext context)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = TvJwtBearer.Issuer,
            ValidAudience = TvJwtBearer.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(TvJwtBearer.SecurityKey)),
            ClockSkew = TimeSpan.Zero 
        };
    }
}