using Static.Results;
using System.Net;

namespace Static.Domain.Errors;

public static class UserErrors
{
    public static readonly Error UnAuthorizedAccess = new(HttpStatusCode.Forbidden,"UnAuthorized to perform this action!");
    public static readonly Error UserNotFound = new(HttpStatusCode.NotFound,"User not found!");
    public static readonly Error UserNotCreated = new(HttpStatusCode.InternalServerError,"User not created!");
    public static readonly Error EmailAlreadyTaken = new(HttpStatusCode.Ambiguous,"This email is already taken!");
    public static readonly Error RoleNotFound = new(HttpStatusCode.BadRequest,"User role is not valid!");
    public static readonly Error InValidEmail = new(HttpStatusCode.Ambiguous,"Invalid Email Address!");
    public static readonly Error ChildAlreadyLinked = new(HttpStatusCode.Ambiguous, "Child is already linked!");
    public static readonly Error ForgotPasswordTokenExpire = new(HttpStatusCode.BadRequest, "Forgot password token expire.Please Try again!");
    public static readonly Error UnAuthorized = new(HttpStatusCode.Unauthorized,"UnAuthorized user!");
    public static readonly Error InValidOtp = new(HttpStatusCode.Unauthorized,"The OTP code is invalid or has expired.Please try again!");
    public static readonly Error ErrorMessageForName = new(HttpStatusCode.Unauthorized,"Name should not contain special characters!");
    public static readonly Error ErrorMessageForNickName = new(HttpStatusCode.Unauthorized,"Nickname should not contain special characters!");
    public static readonly Error ErrorMessageForLimit = new(HttpStatusCode.Unauthorized,"Limit cannot be 0 or negative!");
    public static readonly Error InValidToken = new(HttpStatusCode.Unauthorized,"Invalid Token!");

    public static readonly Error InValidEmailDomain = new(HttpStatusCode.Forbidden,
        "Only email addresses with @innovatetech.co, @innovatetech.io, @innovatetech.com.np domain are allowed.");
}