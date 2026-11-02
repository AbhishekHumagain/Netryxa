using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Configuration;

namespace Static.Infrastructure.Common
{
    public static class ConfigurationExtensions
    {
        private static readonly ILogger _logger = Logger.GetLogger(typeof(ConfigurationExtensions));

        public static T GetValues<T>(this IConfiguration configuration, string name) where T : class
        {
            var values = configuration.GetSection(name).Get<T>();
            if (values == null)
            {
                var ex = new NullReferenceException();
                _logger.Error(ex, $"Configuration Exception at {name}");
                values = configuration.Get<T>();
                return values;
            }
            return values;
        }
    }
}
