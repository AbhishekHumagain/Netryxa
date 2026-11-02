using System.Net;
using Static.Helper.Constants;
using Static.Helper.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Static.API.Filter;

public class AuthorizeNotificationSocketAttribute: Attribute, IAuthorizationFilter
{
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request;

        if (!request.Headers.TryGetValue("Authorization", out StringValues socketToken))
        {
            SendErrorResponse(context, "Authorization header not found!");
            return;
        }
        request.EnableBuffering();
        var isVerified = await VerifyTokenHeader(socketToken);
        if (!isVerified)
        {
            SendErrorResponse(context, "Unauthorized socket token request!");
            return;
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
    private static async Task<bool> VerifyTokenHeader(string receivedSignature)
    {
        try
        {
            await EncryptionDecryption.DecryptString(StaticConstants.WebSocketEncryptionKey,receivedSignature.Replace("Bearer ","").Trim());
            return true;
        }
        catch
        {
            return false;
        }
    }
}