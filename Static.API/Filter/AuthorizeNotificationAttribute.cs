using Static.Helper.Constants;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Static.API.Filter;

public class AuthorizeNotificationAttribute: Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request;

        if (!request.Headers.TryGetValue("notification-token", out StringValues notificationToken))
        {
            SendErrorResponse(context, "notification-token header not found!");
            return;
        }
        

        request.EnableBuffering();
        var isVerified = VerifyTokenHeader(notificationToken);
        if (!isVerified)
        {
            SendErrorResponse(context, "Unauthorized notification callback request!");
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
    
    
    private static bool VerifyTokenHeader(string receivedSignature)
    {
        //For Now Simple Validation 
        return StaticConstants.NotificationToken == receivedSignature;
    }
}