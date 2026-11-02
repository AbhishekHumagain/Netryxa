using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Reflection;

namespace Static.API.Configurations;

/// <summary>
/// The Swagger Configurations
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Service Contain Logic for Swagger
    /// </summary>
    /// <param name="services"></param>
    public static void AddSwaggerService(this IServiceCollection services) =>
        services.AddSwaggerGen(options =>
        {
            //Todo unique readable schema name
            options.CustomSchemaIds(type => type.ToString().Replace("+", "."));

            options.SwaggerDoc("all", new() { Title = "All Static API", Version = "v1" });
            options.SwaggerDoc("api-end-user", new() { Title = "End-User API", Version = "v1" });
            options.SwaggerDoc("api-bo", new() { Title = "Back-Office API", Version = "v1" });

            // FILTER ROUTES BASED ON DOCUMENT NAME
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                return docName switch
                {
                    "all" => true,
                    "api-end-user" => apiDesc.RelativePath!.StartsWith("api/"),
                    "api-bo" => apiDesc.RelativePath!.StartsWith("api-bo/"),
                    _ => false
                };
            });
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.OrderActionsBy((apiDesc) =>
                apiDesc.ActionDescriptor.RouteValues["controller"] + apiDesc.RelativePath);

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            // Define the Bearer scheme — paste your Keycloak JWT token into the Swagger UI
            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "Enter your Keycloak JWT token below.\n\nExample: eyJhbGci...",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

            // Apply the Bearer requirement globally to all endpoints
            options.AddSecurityRequirement((document) => new OpenApiSecurityRequirement()
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });

    /// <summary>
    /// Use Swagger Service.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="configuration"></param>
    public static void UseSwaggerService(this WebApplication app, IConfiguration configuration)
    {
        if (!app.Environment.IsDevelopment() && !configuration.GetValue<bool>("Swagger:Enabled")) return;

        app.UseSwagger(opt =>
        {
            opt.RouteTemplate = "/openapi/{documentName}.json";
        });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/all.json", "All Static API");
            options.SwaggerEndpoint("/openapi/api-bo.json", "Back-Office API");
            options.SwaggerEndpoint("/openapi/api-end-user.json", "End-User API");

            options.IndexStream = () => Assembly.GetExecutingAssembly()
                                                .GetManifestResourceStream(
                                                    "Static.API.wwwroot.swagger.ui.index.html");
        });

        // shall be removed after review
        app.MapScalarApiReference(x =>
        {
            x.WithTitle("Static API V2")
             .WithTheme(ScalarTheme.Default)
             .WithLayout(ScalarLayout.Modern)
             .WithModels(false)
             .WithFavicon("https://cdn.betterttv.net/emote/5590b223b344e2c42a9e28e3/1x.webp")
             .WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.Http11);

            string[] documents = ["all", "api-end-user", "api-bo"];
            x.AddDocuments(documents);
        });
    }
}