using Static.UserSession.CurrentUser.Service;

namespace Static.UserSession.CurrentUser.Guid;

public interface ICurrentUserService : Service.ICurrentUserService<System.Guid>;

public interface ICurrentUserService<TRoleEnum> : ICurrentUserService, ICurrentUserService<System.Guid, TRoleEnum> where TRoleEnum : struct, Enum;