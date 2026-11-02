using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Static.Infrastructure.Common.LocalizationExtensions;

public class LanguageRouteConstraint : IRouteConstraint
{
    public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {

        if (!values.TryGetValue("culture", out var value))
            return false;

        var culture = value.ToString();
        return culture is "en-US" or "fr-FR" or "id-ID";
    }
}