using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Static.Application.Common.Localize;

public class Resource
{
    
}

public static class LocalizationHelper
{
    private static readonly IStringLocalizer Localizer;
    static LocalizationHelper()
    {
        var options = Options.Create(new LocalizationOptions()
        {
            ResourcesPath = ""
        });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        Localizer = factory.Create(typeof(Resource));
    }
    
    public static void SetCulture(string language)
    {
        var culture = new CultureInfo(language);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }
    public static string Get(string key)
    {
        var result = Localizer[key];
        if (!result.ResourceNotFound)
            return result;
        var match = Localizer.GetAllStrings()
                             .FirstOrDefault(s => string.Equals(s.Name, key, StringComparison.OrdinalIgnoreCase));
        return match?.Value ?? key;
    }
}