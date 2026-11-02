using Static.QueryUtils.Pagination.Extensions;
using FluentValidation;
using Static.QueryUtils.Pagination.Filter;

namespace Static.QueryUtils.Pagination.Validators;

public sealed class PaginatedQueryValidator<T> : AbstractValidator<T> where T : class, IPaginationFilter
{
    public PaginatedQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => x.IsPagingDisabled() || (x.Page > 0 && x.Size > 0))
            .WithName(nameof(PaginationFilter))
            .WithMessage("Both 'page' and 'size' must be set to 0 to disable pagination, or both must be greater than 0 to enable it.");

        When(x => x.IsPagingEnabled(), () =>
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("'page' must be greater than 0 when pagination is enabled.");

            RuleFor(x => x.Size)
                .GreaterThan(0)
                .WithMessage("'size' must be greater than 0 when pagination is enabled.");
        });
    }
}