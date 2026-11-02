using Static.Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace Static.API.Filter;

/// <summary>
/// This filter is only to validate the Tv
/// </summary>
public class AuthorizeTvAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// On Authorization Validate the token
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
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