using Static.Application.Common.Interfaces;
using Static.Domain.Entities;
using Static.Domain.Enums;
using Static.EntityFrameworkCore.Repository.Guid;
using Microsoft.AspNetCore.Http;

namespace Static.Infrastructure.Services;

public class LoginAuditService : ILoginAuditService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<UserLoginAttempt> _loginAttemptRepository;

    public LoginAuditService(
        IHttpContextAccessor httpContextAccessor,
        IGenericRepository<UserLoginAttempt> loginAttemptRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _loginAttemptRepository = loginAttemptRepository;
    }

    public async Task LogLoginAttemptAsync(string usernameOrEmail, LoginResultType result, Guid? userId = null)
    {
        var context = _httpContextAccessor.HttpContext;
        var attempt = new UserLoginAttempt
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserNameOrEmailAddress = usernameOrEmail,
            ClientIpAddress = context?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            BrowserInfo = context?.Request.Headers["User-Agent"].ToString() ?? "Unknown",
            Result = result,
            CreationTime = DateTime.UtcNow
        };

        await _loginAttemptRepository.InsertAsync(attempt);
    }
}
