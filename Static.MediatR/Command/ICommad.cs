using Static.Results;
using MediatR;

namespace Static.MediatR.Command;

public interface IBaseCommand;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

/// <summary>
/// Marks a command that must not run inside the unit-of-work transaction — typically because it
/// waits on an external service (e.g. Keycloak) that could outlive the transaction timeout.
/// Any database work it needs should be delegated to a separate, regular command.
/// </summary>
public interface INonTransactionalCommand;
