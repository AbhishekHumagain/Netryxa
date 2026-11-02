using Static.Helper.Constants.EnumCollections;

namespace Static.Domain.Exceptions
{
    public class UnsupportedUserRoleException(StaticRole staticRole)
        : Exception($"Role of type \"{staticRole.ToString()}\"is unsupported.");
}
