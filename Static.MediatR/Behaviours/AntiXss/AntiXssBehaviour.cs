using Static.UserSession.CurrentUser.Service;
using Ganss.Xss;
using MediatR;
using Microsoft.Extensions.Logging;
using Static.MediatR.Command;
using System.Reflection;

namespace Static.MediatR.Behaviours.AntiXss;

public class AntiXssBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> 
    where TResponse : IBaseCommand
{
    public virtual async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        #region Santize Request

        if (AntiXss.CommandWhitelist.Contains(typeof(TRequest).Name))
        {
            return await next(cancellationToken);
        }

        var sanitizer = new HtmlSanitizer();

        var requestedCommand = request.GetType().Name;

        var properties = request.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.PropertyType == typeof(string));
        foreach (var property in properties)
        {
            if (property.GetValue(request) is not string value) continue;
            if (!IsXssPattern(value)) continue;

            var sanitizedValue = sanitizer.Sanitize(value);
            //var encodedInput = HttpUtility.HtmlEncode(sanitizedValue); //Commented because it's giving issue in rich-text input
            property.SetValue(request, sanitizedValue);
            logger.LogWarning("[Anti XSS Detection] XSS detected in command: {RequestedCommand} by user: {UserId}.", requestedCommand, currentUserService.UserId);
        }

        #endregion


        return await next(cancellationToken);
    }

    #region XSS pattern validator

    private static bool IsXssPattern(string value) =>
        RegexPattern.Xss().IsMatch(value) ||
        RegexPattern.Html().IsMatch(value) ||
        RegexPattern.SpecialChars().IsMatch(value);

    #endregion
}