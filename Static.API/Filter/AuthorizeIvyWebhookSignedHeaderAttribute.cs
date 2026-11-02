using Static.Helper.Constants;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Static.API.Filter;

public class AuthorizeIvyWebhookSignedHeaderAttribute : Attribute, IAuthorizationFilter
{
    public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Headers.TryGetValue("IVy-Signature", out var ivySignature))
            {
                SendErrorResponse(context, "IVy-Signature header not found!");
                return;
            }

            request.EnableBuffering();

            var stringPayload = await GetRequestBodyString(request.Body);
            var isVerified = VerifySignedHeader(stringPayload, ivySignature.ToString());
            if (!isVerified)
            {
                SendErrorResponse(context, "Unauthorized Ivy webhook request");
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

        private static bool VerifySignedHeader(string payload, string receivedSignature)
        {
            var webhookSecret = StaticConstants.IvyWebhookSecret;
            var computedHmac = HmacDigest(payload, webhookSecret);
            return computedHmac == receivedSignature;
        }

         private static async Task<string> GetRequestBodyString(Stream body)
         {
             var reader = new StreamReader(body);
             var payload = await reader.ReadToEndAsync();
             return payload;
         }

         private static string HmacDigest(string content, string secretKey)
         {
             var encoding = new UTF8Encoding();
             var keyBytes = encoding.GetBytes(secretKey);
             var contentBytes = encoding.GetBytes(content);
             var hashMessage = new HMACSHA1(keyBytes).ComputeHash(contentBytes);
             return string.Concat(Array.ConvertAll(hashMessage, x => x.ToString("x2")));
         }
}

