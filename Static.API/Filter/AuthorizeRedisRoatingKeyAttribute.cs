using System.Net;
using System.Security.Cryptography;
using System.Text;
using Static.Application.Common.Dto;
using Static.Application.Common.Interfaces;
using Static.Helper.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Static.API.Filter;

public class AuthorizeRedisRoatingKeyAttribute : Attribute, IAuthorizationFilter
{
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request;
        if (!request.Headers.TryGetValue("Callback-Signature", out var callbackSignature))
        {
            SendErrorResponse(context, "Header not found!");
            return;
        }
        
        var getRedisclient = context.HttpContext.RequestServices.GetService<ICacheService>();
        if (getRedisclient == null)
        {
            SendErrorResponse(context, "Cache service down.");
            return;
        }
        var getValue = getRedisclient.GetData<RedisRoatingKeyDto>(StaticConstants.RedisKeys.Api);

        if (getValue == null)
        {
            SendErrorResponse(context, "No keys found.");
            return;
        }
        
        request.EnableBuffering();
        
        string[] tokenParts = callbackSignature.ToString().Split('.');

        if (tokenParts.Length != 3)
        {
            SendErrorResponse(context, "Invalid token format");
            return;
        }
        
        var isVerifiedCurrentKey= VerifySignedHeader(getValue.CurrentKey, tokenParts);
        if (!isVerifiedCurrentKey)
        {
            var isVerifiedPreviousKey = VerifySignedHeader(getValue.PreviousKey, tokenParts);
            if (!isVerifiedPreviousKey)
            {
                SendErrorResponse(context, "Unauthorized callback request");
                return;
            }
        }
        request.Body.Position = 0;
    }
    
    private static void SendErrorResponse(AuthorizationFilterContext context, string message)
    {
        context.Result = new ContentResult()
        {
            StatusCode = (int) HttpStatusCode.Unauthorized,
            Content = message
        };
    }
    
    private static bool VerifySignedHeader(string secretKey, string[] token)
    {
        string header = token[0];
        string payload = token[1];
        string receivedSignature = token[2];
        
        string unsignedToken = string.Join('.', new string[] { header, payload });
        
        try
        {
            // Create a new instance of HMACSHA256 with the secret key
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedToken));
                string computedSignature = Convert.ToBase64String(hash)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');
                if (computedSignature == receivedSignature)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);

        }
      
    }
    
}