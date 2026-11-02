using System.Text.RegularExpressions;

namespace Static.MediatR.Behaviours.AntiXss;

internal static partial class RegexPattern
{
    [GeneratedRegex(@"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", RegexOptions.IgnoreCase)]
    public static partial Regex Xss();

    [GeneratedRegex("<[^>]+>", RegexOptions.IgnoreCase)]
    public static partial Regex Html();

    [GeneratedRegex(@"[^\w\s]", RegexOptions.IgnoreCase)]
    public static partial Regex SpecialChars();
}