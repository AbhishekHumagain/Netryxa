using Static.Helper.Constants;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Static.API.Filter;

public class AuthorizeTestpaperCallBackAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request;

        if (!request.Headers.TryGetValue("testpaper-token", out StringValues testpaperToken))
        {
            SendErrorResponse(context, "testpaper-token header not found!");
            return;
        }
        

        request.EnableBuffering();
        var isVerified = VerifyTokenHeader(testpaperToken);
        if (!isVerified)
        {
            SendErrorResponse(context, "Unauthorized test paper callback request!");
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
        return StaticConstants.TestpaperToken.ToString() == receivedSignature;
    }
}