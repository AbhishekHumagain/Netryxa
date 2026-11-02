using Static.Helper.Constants.EnumCollections;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Static.Domain.Entities;


namespace Static.Domain.Utils;

public static class StaticUtlExtensions
{
    public static async Task<T?> DeserializeDefaultSetting<T>(this IQueryable<DefaultSetting> defaultSetting,
        string staticSettings) where T : class
    {
        var value = await defaultSetting.FirstOrDefaultAsync(x => x.Name.ToLower() == staticSettings.ToLower());
        if (value is null) return null;
        return JsonConvert.DeserializeObject<T>(value?.Value!)!;
    }
    
    public static StaticRole GetRoleEnum(string roleName) =>
        Enum.TryParse<StaticRole>(roleName, out var role)
            ? role
            : throw new InvalidOperationException("Invalid Role.");
    
    /// <summary>
    /// Checks a value is between a minimum and maximum value.
    /// </summary>
    /// <param name="value">The value to be checked</param>
    /// <param name="minInclusiveValue">Minimum (inclusive) value</param>
    /// <param name="maxInclusiveValue">Maximum (inclusive) value</param>
    
    public static bool IsBetween<T>(this T value, T minInclusiveValue, T maxInclusiveValue) where T : IComparable<T>
    {
        return value.CompareTo(minInclusiveValue) >= 0 && value.CompareTo(maxInclusiveValue) <= 0;
    }
    
    public static string ToUtcFormat(this DateTime dateTime) => dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
    public static string ToMonthDayYearFormat(this DateTime dateTime) => dateTime.Date.ToString("MMM dd yyyy");
    
    public static string ToHourMinutesFormat(this DateTime dateTime) => dateTime.ToString("HH:mm");
    
    public static string ToMinutesFormat(this DateTime dateTime) => dateTime.ToString("mm");
    
    public static string ToWeekMonthDayYearFormat(this DateTime dateTime) => dateTime.ToString("ddd MMM dd yyyy");
    public static string ToDayMonthYearFormat(this DateTime dateTime) => dateTime.ToString("dd/MM/yyyy");
    public static string ToMonthDayYearFormatSlash(this DateTime dateTime) => dateTime.ToString("MM/dd/yyyy");
    
    public static string ToMonthNameDayYearTime(this DateTime dateTime) => dateTime.ToString("MMMM dd,yyyy, hh:mm tt");

    public static string ToTime(this DateTime dateTime) => dateTime.ToString("h:mm tt");
}