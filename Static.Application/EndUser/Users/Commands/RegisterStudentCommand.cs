using FluentValidation;
using Static.MediatR.Command;

namespace Static.Application.EndUser.Users.Commands;

/// <summary>
/// Registers a new student: creates the user in Keycloak with the "Student" realm role
/// and returns the Keycloak user ID for subsequent local-profile creation.
/// </summary>
public sealed record RegisterStudentCommand(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password) : ICommand<string>;

public sealed class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
{
    public RegisterStudentCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
