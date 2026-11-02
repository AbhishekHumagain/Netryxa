using Static.Helper.Constants.EnumCollections;

namespace Static.Application.Common.CurrentUserService;

public interface ICurrentUserService : UserSession.CurrentUser.Guid.ICurrentUserService<StaticRole>
{
}