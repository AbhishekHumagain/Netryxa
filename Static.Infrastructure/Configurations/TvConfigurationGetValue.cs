namespace Static.Infrastructure.Configurations
{
    /// <summary>
    /// This class get and set the value of tv config
    /// </summary>
    public class TvConfigurationGetValue
    {
        /// <summary>
        /// This will get value from configurations
        /// </summary>
        public static TvJwtBearer JwtBearer { get; set; }
    }

    /// <summary>
    /// This class will get configuration value inside the Tv configurations
    /// </summary>
    public class TvJwtBearer
    {
        /// <summary>
        /// its hold the raw string value of securityKey
        /// </summary>
        public static string SecurityKey { get; set; }
    
        /// <summary>
        /// It holds the issuer value
        /// </summary>
        public static string Issuer { get; set; }
        /// <summary>
        /// It holds audience value
        /// </summary>
        public static string Audience { get; set; }
        /// <summary>
        /// It holds isEnable settings
        /// </summary>
        public static bool IsEnabled { get; set; }
        /// <summary>
        /// It holds the life-time of expirations
        /// </summary>
        public static int Expiration { get; set; }
        /// <summary>
        /// It holds the default-scheme 
        /// </summary>
        public static string DefaultScheme { get; set; }
    
    }
}
