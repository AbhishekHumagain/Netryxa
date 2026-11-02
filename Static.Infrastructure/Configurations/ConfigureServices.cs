using Amazon.Runtime;
using Amazon.S3;
using Hangfire;
using Hangfire.PostgreSql;
using Static.Application.Common.Interfaces;
using Static.Application.Common.Interfaces.Shared;
using Static.Application.Common.Interfaces.Shared.CloudFlare;
using Static.Application.Common.Interfaces.Shared.CloudFront;
using Static.Cryptography;
using Static.DateTimeProvider;
using Static.EntityFrameworkCore.Repository.Guid;
using Static.EntityFrameworkCore.UnitOfWork;
using Static.Helper.Constants;
using Static.Identity.Extensions;
using Static.Infrastructure.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Static.Infrastructure.Persistence;
using Static.Infrastructure.Persistence.Interceptors;
using Static.Infrastructure.Persistence.Seed;
using Static.Infrastructure.Services;
using Static.Infrastructure.Services.AmazonService;
using Static.Infrastructure.Services.AmazonService.CloudFront;
using Static.Infrastructure.Services.AmazonService.S3;
using Static.Infrastructure.Services.AmazonService.SNS;
using Static.Infrastructure.Services.AmazonService.SQS;
using Static.Infrastructure.Services.Cache;
using Static.Infrastructure.Services.CloudFlareService;
using Static.Infrastructure.Services.RabbitMqService;
using Static.Infrastructure.Services.CodeExecution;
using Static.Application.Common.Interfaces;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Static.Infrastructure.Configurations;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration,
        bool isProduction)
    {
        services.AddDateTimeProvider();

        #region Service and Connection

        services.AddDbContext<StaticDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(StaticDbContext).Assembly.FullName));

                if (!isProduction && configuration.GetValue<bool>("Debugging:EnableSensitiveDataLogging"))
                {
                    options.EnableSensitiveDataLogging();
                }
            }
        );

        configuration.GetValues<RabbitMqConfiguration>("RabbitMQConnection");

        services.AddSingleton<IAwsConfiguration>(configuration.GetValues<AwsConfiguration>("AWS"));

        // Register IAmazonS3 manually using credentials
        services.AddSingleton<IAmazonS3>(provider =>
        {
            var config = provider.GetRequiredService<IAwsConfiguration>();
            var credentials = new BasicAWSCredentials(config.AccessKey, config.SecretKey);
            var awsRegionEndpoint = StaticConstants.AwsRegionEndpoint;

            return new AmazonS3Client(credentials, awsRegionEndpoint);
        });

        services.AddSingleton<IGoogleRecaptchaConfiguration>(
            configuration.GetValues<GoogleRecaptchaConfiguration>("GoogleRecaptcha"));

        // var redisConnectionString = configuration.GetConnectionString("Redis");
        // var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString!);
        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.InstanceName = "Static:";
        //     options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(connectionMultiplexer);
        // });
        // services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

        services.AddSingleton<IEmailValidationConfiguration>(
            configuration.GetValues<EmailValidationConfiguration>("Email"));


        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddHangfire(globalConfiguration => globalConfiguration
                                                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                                                    .UseSimpleAssemblyNameTypeSerializer()
                                                    .UseRecommendedSerializerSettings()
                                                    .UseFilter(new AutomaticRetryAttribute
                                                    {
                                                        Attempts = 1,
                                                        OnAttemptsExceeded = AttemptsExceededAction.Delete
                                                    })
                                                    .UsePostgreSqlStorage(x =>
                                                        {
                                                            x.UseNpgsqlConnection(connectionString);
                                                        },
                                                        new()
                                                        {
                                                            DistributedLockTimeout = TimeSpan.FromMinutes(1),
                                                            PrepareSchemaIfNecessary = true
                                                        })
        );

        #endregion

        #region Self Configuration

        services.AddAuthService(configuration, isProduction);
        // Keycloak OIDC services (IKeycloakService, IKeycloakAdminService) + JWT Bearer validation
        // isProduction controls RequireHttpsMetadata on the JWT handler (true = enforce HTTPS)
        services.AddKeycloakIdentity(configuration, isProduction);

        #endregion

        #region Injected Services

        #region Scoped

        services.AddScoped<AuditingSaveChangesInterceptor>();
        services.AddScoped<IStaticDbContext>(provider => provider.GetRequiredService<StaticDbContext>());
        var seeders = Assembly.GetExecutingAssembly()
                              .GetTypes()
                              .Where(t => typeof(ISeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var seeder in seeders)
        {
            services.AddScoped(typeof(ISeeder), seeder);
        }

        services.AddScoped<StaticDbContextInitializer>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IRabbitMQProducer, RabbitMqProducer>();
        services.AddScoped<ILoginAuditService, LoginAuditService>();

        #endregion

        #region Transient

        services.AddTransient<IEmailQueueService, EmailQueueService>();
        services.AddTransient<ISQSService, SQSService>();
        services.AddTransient<IS3Service, S3Service>();
        services.AddTransient<ICloudFrontService, CloudFrontService>();
        services.AddTransient<ICloudFlareService, CloudFlareService>();
        services.AddTransient<ISnsService, SNSService>();
        services.AddTransient<IRedisService, RedisService>();
        services.AddTransient<IEmailValidationService, EmailValidationService>();
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        //for payment gateway adapter using an adapter pattern
        #endregion

        #region Singleton
        

        //services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IS3ClientFactory, S3ClientFactory>();

        #endregion

        services.AddHttpClient();

        services.AddScoped<ICodeExecutionService, LocalPythonExecutionService>();

        #endregion

        services.AddSharedService();

        #region Encryption

        CipherDefaults.AesSha256Defaults.PassPhrase = configuration["Encryption:PassPhrase"]!;
        CipherDefaults.AesSha256Defaults.Salt = Encoding.ASCII.GetBytes(configuration["Encryption:Salt"]!);
        CipherDefaults.AesSha256Defaults.KeySize = configuration.GetValue<int>("Encryption:KeySize");
        CipherDefaults.AesSha256Defaults.InitVectorBytes =
            Encoding.ASCII.GetBytes(configuration["Encryption:InitVector"]!);

        #endregion

        #region CloudFlare Client

        services.AddHttpClient(StaticConstants.HttpClients.CloudFlare, c =>
        {
            c.BaseAddress = new Uri(configuration["CloudFlare:BaseUrl"]!);
            c.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", configuration["CloudFlare:Token"]!);
        });

        #endregion

        #region EventingSystem Event

        services.AddHttpClient(StaticConstants.HttpClients.EventingSystem, c =>
        {
            c.BaseAddress = new Uri(configuration["EventingSystem:BaseUrl"]!);
            c.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", configuration["EventingSystem:Token"]!);
        });

        #endregion

        #region Reporting Client

        services.AddHttpClient(StaticConstants.HttpClients.Reporting, c =>
        {
            c.BaseAddress = new Uri(configuration["Reporting:BaseUrl"]!);
            c.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", configuration["Reporting:Token"]!);
        });

        #endregion

        #region PlantUML Client

        services.AddHttpClient(StaticConstants.HttpClients.PlantUml, c =>
        {
            c.BaseAddress = new Uri(configuration["PlantUml:BaseUrl"]!);
        });

        services.AddTransient<IPlantUmlService, PlantUmlService>();

        #endregion

        return services;
    }

    public static async Task UseInfrastructureServices(this WebApplication app)
    {
        if (!app.Configuration.GetValue("Database:InitializeOnStartup", app.Environment.IsDevelopment())) return;
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<StaticDbContextInitializer>();
        await initializer.InitialiseAsync();
        var seeders = scope.ServiceProvider.GetServices<ISeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }
    }
}