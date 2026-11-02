using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Static.QueryUtils.Pagination.Validators;

public static class PaginatedQueryValidatorRegistrar
{
    public static IServiceCollection AddPaginatedQueryValidators(this IServiceCollection services) => services.AddScoped(typeof(IValidator<>), typeof(PaginatedQueryValidator<>));
}