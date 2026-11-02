using Microsoft.AspNetCore.Http;
using NodaTime;
using NodaTime.Text;
using Static.Helper.Constants;
using Static.Helper.Constants.EnumCollections;
using System.Text.RegularExpressions;


namespace Static.Helper.Utils;

public static class StaticUtlExtensions
{
    public static decimal MoneyValidationChecker(this decimal amount)
    {
        var stringAmount = amount.ToString();
        const string pattern =
            @"(?<=\s|^)(?:(?:(?:\d{1,3}(?:,\d{3})*)|\d+)(?:\.\d{1,2})?|\$\d+(?:,\d{3})*(?:\.\d{2})?|\€\d+(?:,\d{3})*(?:\.\d{2})?|\£\d+(?:,\d{3})*(?:\.\d{2})?|\d+(?:\.\d{1,2})?)(?=\s|$)";
        if (Regex.IsMatch(stringAmount, pattern))
        {
            return Convert.ToDecimal(amount);
        }
        else
        {
            throw new FormatException("Invalid Amount");
        }
    }


    public static string GetTXNIdWithHiddenCharacter(this string transactionId)
    {
        //AsSpan allows to work with the characters of the string without creating a new string instance.
        return string.Concat("XXXX-", transactionId.AsSpan(transactionId.Length - 4));
    }

    public static string GetDomainName(this IHttpContextAccessor httpContextAccessor)
    {
        var url = httpContextAccessor.HttpContext?.Request.Host.Value;
        var nodes = url.Split('.');
        var lastNodeIndex = nodes.Length - 1;
        var domain = string.Empty;
        if (lastNodeIndex > 0)
        {
            domain = nodes[lastNodeIndex - 1] + "." + nodes[lastNodeIndex];
        }

        return domain;
    }

    public static StaticConstants.ResourceAccessType GetResourceAvailability(bool hasResourceForStudent,
        bool hasResourceForTeacher)
    {
        if (hasResourceForStudent && hasResourceForTeacher)
        {
            return StaticConstants.ResourceAccessType.ParentStudent;
        }

        if (hasResourceForStudent)
        {
            return StaticConstants.ResourceAccessType.Student;
        }

        if (hasResourceForTeacher)
        {
            return StaticConstants.ResourceAccessType.Parent;
        }

        return StaticConstants.ResourceAccessType.None;
    }

    public static async Task<List<Guid>> GetShuffledSuggestionList(
        List<Guid> request)
    {
        var randomOrderList = request.OrderBy(x => Guid.NewGuid().ToString()).ToList();

        for (int i = 0; i < request.Count; i++)
        {
            request[i] = randomOrderList[i];
        }

        return request;
    }

    public static bool IsThreeConsecutiveSequences(this string number)
    {
        if (number.Length < 3)
        {
            return false;
        }

        for (int i = 0; i <= number.Length - 3; i++)
        {
            bool isConsecutive = true;
            for (int j = 1; j < 3; j++)
            {
                if (number[i + j] != number[i] + j)
                {
                    isConsecutive = false;
                    break;
                }
            }

            if (isConsecutive)
            {
                return true;
            }
        }

        return false;
    }

    public static string GetMasteryStatus(this decimal mastery)
    {
        return mastery switch
        {
            100 => StaticConstants.MasteryStatus.Mastered,
            >= 90 and < 100 => StaticConstants.MasteryStatus.AdequateMastery,
            >=0 and <=90 => StaticConstants.MasteryStatus.InadequateMastery,
            _ => StaticConstants.MasteryStatus.NA
        };
    }

    public static bool IsXSSPattern(string value)
    {
        var xssPattern = @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>";
        var htmlPattern = @"<[^>]+>";
        var specialCharsPattern = @"[^\w\s]";
        if (Regex.IsMatch(value, xssPattern, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(value, htmlPattern, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(value, specialCharsPattern, RegexOptions.IgnoreCase))
        {
            return false;
        }

        return true;
    }
    
    public static Guid? ToNullableGuid(this string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        return Guid.TryParse(value, out var result) ? result : null;
    }

    public static Guid ToGuid(this string value)
    {
        return Guid.Parse(value);
    }
    
    public static bool HasRole(this StaticRole userRole, string userType)
    {
        return userRole.ToString().Equals(userType, StringComparison.OrdinalIgnoreCase);
    }
    
    public static (string Name, string Surname) ResolveNameAndSurnameFrom(string fullName)
    {
        var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return (names[0], string.Join(" ", names.Skip(1)));
    }
    
    public static (string name, string surname) ResolveNameAndSurnameFrom(string? name, string? surname, string? fullName)
    {
        if (!string.IsNullOrWhiteSpace(fullName)) return ResolveNameAndSurnameFrom(fullName);
        if (name == null) throw new InvalidOperationException("Name and Surname or Full name is required.");
        return (name, surname ?? string.Empty);
    }
    public static string RoleToString(this int userRole)
    {
        return Enum.GetName(typeof(StaticRole), userRole) ?? string.Empty;
    }
    
    public static decimal ToDecimal(this double value)
    {
        if (double.IsNaN(value))
        {
            return 0;
        }
        return double.IsInfinity(value) ? 0 : Convert.ToDecimal(value);
    }
    
    public static decimal ToDecimal(this float value)
    {
        if (double.IsNaN(value))
        {
            return 0;
        }
        return double.IsInfinity(value) ? 0 : Convert.ToDecimal(value);
    }
    
    public static bool PasswordValidator(string password)
    {
        var lowercase = new Regex("[a-z]+");
        var uppercase = new Regex("[A-Z]+");
        var digit = new Regex("(\\d)+");
        var symbol = new Regex("(\\W)+");

        return (lowercase.IsMatch(password) && uppercase.IsMatch(password) && digit.IsMatch(password) &&
                symbol.IsMatch(password));
    }
    
    /// <summary>
    /// Converts the specified UTC <see cref="DateTime"/> to a local time in the given IANA time zone (e.g., "Asia/Kathmandu").
    /// This method should only be used internally in scenarios such as generating reports or logs where the UTC offset is not explicitly provided.
    /// It assumes the input <paramref name="dateTime"/> is in UTC and returns a <see cref="DateTime"/> in the local time of the specified time zone without preserving the offset.
    /// </summary>
    /// <param name="dateTime">The UTC DateTime to convert.></param>
    /// <param name="timeZone">The IANA time zone ID (e.g., "Asia/Kathmandu").</param>
    /// <returns>The corresponding local time as a DateTime in the specified time zone, or the original DateTime if the time zone is invalid or parsing fails.</returns>
    public static DateTime FromTimeZone(this DateTime dateTime, string timeZone)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
        {
            return dateTime;
        }
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZone);
        if (zone == null)
            return dateTime;
        const string dateFormat = "dd/MM/yyyy HH:mm:ss";
        var pattern = InstantPattern.CreateWithInvariantCulture(dateFormat);
        var parseResult = pattern.Parse(dateTime.ToString(dateFormat));
        return !parseResult.Success ? dateTime : parseResult.Value.InZone(zone).ToDateTimeUnspecified();
    }
}