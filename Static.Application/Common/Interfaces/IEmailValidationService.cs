namespace Static.Application.Common.Interfaces;

public interface IEmailValidationService
{
    Task<bool> ValidateEmailAddress(string emailAddress);
}