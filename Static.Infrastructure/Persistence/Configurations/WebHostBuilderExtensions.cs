using Microsoft.AspNetCore.Hosting;

namespace Static.Infrastructure.Persistence.Configurations
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureEnvironmentVariablesFromDotEnvFile(this IWebHostBuilder hostBuilder)
        {
            DotNetEnv.Env.NoClobber().Load();
            return hostBuilder;
        }
    }
}
