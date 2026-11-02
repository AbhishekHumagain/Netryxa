using Static.Infrastructure.Configurations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace Static.API.Filter;

/// <summary>
/// This filter is only to validate the WebSocket server
/// </summary>
public class AuthorizeWssFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            var httpContext = context.HttpContext;
            var token = GetTokenFromHeader(httpContext);
            
            if (string.IsNullOrEmpty(token))
            {
                return TypedResults.Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();
            
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                return TypedResults.Unauthorized();
            }

            httpContext.User = principal;
            return await next(context); // Continue execution if authorized
        }
        catch (SecurityTokenException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception)
        {
            return TypedResults.Unauthorized();
        }
    }

    private static string? GetTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader) &&
            authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return null;
    }
    
    private static TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = WssJwtBearer.Issuer,
            ValidAudience = WssJwtBearer.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(WssJwtBearer.SecurityKey)),
            ClockSkew = TimeSpan.Zero 
        };
    }
    
}


