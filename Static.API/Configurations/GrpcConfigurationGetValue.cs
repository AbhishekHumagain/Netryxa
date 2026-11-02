namespace Static.API.Configurations;

/// <summary>
/// This class is made for getting value from appsettings.
/// </summary>
public class GrpcConfigurationGetValue
{
    public static string IvyToken { get; set; }
    public static string IvyUrl { get; set; }
    
    public static string EBookUrl { get; set; }
    public static string EBookToken { get; set; }
    
    public static string TestpaperUrl { get; set; }
    public static string TestpaperToken { get; set; }

    public static string TestpaperUserUrl { get; set; }
    
    public static string TestpaperUserToken { get; set; }
    
    public static string IdentityUrl { get; set; }
}