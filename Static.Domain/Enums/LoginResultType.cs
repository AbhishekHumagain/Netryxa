namespace Static.Domain.Enums;

public enum LoginResultType
{
    Success = 1,
    InvalidUserNameOrEmailAddress,
    InvalidPassword,
    UserIsNotActive,
    UserEmailIsNotConfirmed,
    UnknownExternalLogin,
    LockedOut,
    UserPhoneNumberIsNotConfirmed
}