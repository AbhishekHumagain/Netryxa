using Static.Results;
using Static.UserSession.CurrentUser.Service;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Static.MediatR.Behaviours.Validation;

public class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ICurrentUserService currentUserService,
    ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    public virtual async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next(cancellationToken);
        
        var context = new ValidationContext<TRequest>(request);
        
        var validationErrors = new List<ValidationFailure>();
        
        // To avoid any Concurrency Exception, mainly thrown by DbContext.
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            if (result.IsValid) continue;
            validationErrors.AddRange(result.Errors);
        }

        if (validationErrors.Count == 0) return await next(cancellationToken);
            
        var ex = new ValidationException("One or more validation failed.", validationErrors);
        logger.LogWarning(ex, "[Command Validation] Command validation failed for {RequestName}, User Id = {UserId}.", typeof(TRequest).Name, currentUserService.UserId);
        throw ex;
    }
}