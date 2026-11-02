using Static.Domain.Enums;

namespace Static.Application.Common.Interfaces;

public interface ILoginAuditService
{
    Task LogLoginAttemptAsync(string usernameOrEmail, LoginResultType result, Guid? userId = null);
}