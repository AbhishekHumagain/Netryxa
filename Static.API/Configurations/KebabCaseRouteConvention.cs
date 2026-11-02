using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace Static.API.Configurations;

/// <summary>
/// Defines a route convention that transforms route tokens into kebab-case.
/// </summary>
public class KebabCaseRouteConvention : RouteTokenTransformerConvention
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KebabCaseRouteConvention"/> class
    /// using the <see cref="KebabCaseRouteConvention"/>.
    /// </summary>
    public KebabCaseRouteConvention() : base(new KebabCaseParameterTransformer()) { }
}

internal partial class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null) return null;
        var @string = value.ToString();
        return string.IsNullOrEmpty(@string)
            ? null
            : CaseBoundaryRegex().Replace(@string, "-").ToLower();
    }

    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex CaseBoundaryRegex();
}