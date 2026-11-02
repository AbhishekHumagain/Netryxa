using Microsoft.Extensions.DependencyInjection;

namespace Static.DateTimeProvider;

/// <summary>
/// Provides extension methods for registering the <see cref="IDateTimeProvider"/> service.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds the default <see cref="IDateTimeProvider"/> implementation 
    /// (<see cref="DateTimeProvider"/>) to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the provider is added.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDateTimeProvider(this IServiceCollection services) =>
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
}