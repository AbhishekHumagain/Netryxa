using System.Text.RegularExpressions;

namespace Static.Application.Common;

public static partial class CustomRegex
{
    [GeneratedRegex(@"^[^!@#$%^&*()_+{}[\]|:;<>,.?~/\-]*$")]
    public static partial Regex NameRegex();

    [GeneratedRegex(@"^[/+]?(\d{10,15})$")]
    public static partial Regex PhoneNumber();
    
    [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]+$")]
    public static partial Regex EmailAddress();
    
    [GeneratedRegex(@"[^a-zA-Z0-9_.]+")]
    public static partial Regex CleanFileNameRegex();
}