namespace Static.Infrastructure.Services.RabbitMqService;

internal class RabbitMqConfiguration
{
    public static string UserName { get; set; }
    public static string Password { get; set; }
    public static string HostName { get; set; }
    
    public static string EmailFrom { get; set; }
    public static int Port { get; set; }
    
    public static string SocketNamespace { get; set; }
}