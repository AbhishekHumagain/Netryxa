// using System.Net;
// using System.Security.Claims;
// using Static.Domain.Entities;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
// using Microsoft.IdentityModel.Tokens;
// using IAuthenticationService = Static.Application.Common.Interfaces.IAuthenticationService;
//
// namespace Static.API.Filter;
// public class AuthorizeForgotPasswordTokenAttribute : Attribute  ,IAuthorizationFilter, IAuthorizeData
// {
//     
//     public string? Policy { get; set; }
//     public string? Roles { get; set; }
//     public string? AuthenticationSchemes { get; set; }
//     
//     public AuthorizeForgotPasswordTokenAttribute()
//     {
//         
//     }
//     public AuthorizeForgotPasswordTokenAttribute(string policy) => this.Policy = policy;
//     
//     public void OnAuthorization(AuthorizationFilterContext context)
//     {
//         var request = context.HttpContext.Request;
//         var authenticationService = context.HttpContext.RequestServices.GetService<IAuthenticationService>();
//         
//         var email = context.HttpContext?.User?.FindFirstValue(StaticConstants.CustomClaimType.Email);
//         var jwtToken = context.HttpContext?.GetTokenAsync(StaticConstants.AuthorizedAccessToken).Result;
//         if (!jwtToken.IsNullOrEmpty())
//         {
//             var decodeJwt =  authenticationService?.ReadJwtAccessTokenOnForgotPassword(jwtToken!.Trim());
//             if (!email.IsNullOrEmpty())
//             {
//                 var userManager = context.HttpContext!.RequestServices.GetService<UserManager<User>>();
//                 var options =  userManager!.Options.Tokens.PasswordResetTokenProvider;
//                 var user =  userManager.FindByEmailAsync(email).Result;
//                 if (user == null)
//                 {
//                     SendErrorResponse(context);
//                     return;
//                 }
//           
//                 var verifyToken = userManager.VerifyUserTokenAsync(user,options , StaticConstants.TokenPurpose.PasswordResetToken,decodeJwt).Result;
//                 if (!verifyToken)
//                 {
//                     SendErrorResponse(context);
//                     return;
//                 }
//                 return;
//             } 
//         }
//
//         SendErrorResponse(context);
//         return;
//     }
//     
//     private static void SendErrorResponse(AuthorizationFilterContext context)
//     {
//         context.Result = new ContentResult()
//         {
//             StatusCode = (int) HttpStatusCode.Unauthorized,
//         };
//     }
//     
// }