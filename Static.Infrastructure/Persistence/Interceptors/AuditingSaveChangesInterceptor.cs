using Static.DateTimeProvider;
using Static.EntityFrameworkCore.Interceptors;
using Static.UserSession.CurrentUser.Guid;

namespace Static.Infrastructure.Persistence.Interceptors;

public class AuditingSaveChangesInterceptor(
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTime) : AuditingSaveChangesInterceptor<Guid>(dateTime, currentUserService);
