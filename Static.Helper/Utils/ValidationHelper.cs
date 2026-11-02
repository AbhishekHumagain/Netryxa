using Static.Helper.Constants;
using System.Text.RegularExpressions;

namespace Static.Helper.Utils;

public static class ValidationHelper
{
    /// <summary>
    /// Checks if the provided input is a valid email address.
    /// </summary>
    /// <param name="input">The string to validate.</param>
    /// <returns>True if the input is a valid email; otherwise, false.</returns>
    public static bool IsEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        try
        {
            // Using built-in .NET email validation
            var emailAddress = new System.Net.Mail.MailAddress(input);
            return emailAddress.Address == input;
        }
        catch
        {
            return false;
        }
    }
    
    private static bool ValidateUserName(string userName, string emailAddress)
    {
        if (string.IsNullOrEmpty(userName)) return true;

        // Validate the username is not an email unless it matches the provided email address
        return !IsEmail(userName) || userName.Equals(emailAddress, StringComparison.OrdinalIgnoreCase);
    }
    
    private static bool PasswordValidator(string password)
    {
        var lowercase = new Regex("[a-z]+");
        var uppercase = new Regex("[A-Z]+");
        var digit = new Regex("(\\d)+");
        var symbol = new Regex("(\\W)+");
    
        return (lowercase.IsMatch(password) && uppercase.IsMatch(password) && digit.IsMatch(password) && symbol.IsMatch(password));
    }
    
    private static bool EmailHasValidDomain(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return false;

        // Allowed domains
        var validDomains = StaticConstants.InnovateTechDomains;

        // Check if the email ends with one of the allowed domains
        return validDomains.Any(domain => emailAddress.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
    }
}