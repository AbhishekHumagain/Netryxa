using Microsoft.AspNetCore.Mvc;
using Static.API.Controllers.Helper;
using Static.Application.EndUser.Users.Commands;

namespace Static.API.Controllers.EndUser.Users;

/// <summary>
/// User-registration endpoints: enrol students and teachers.
/// Creates the user in Keycloak and returns the new Keycloak user ID
/// which the caller uses to create the matching local profile.
/// </summary>
[Route("api/users")]
public sealed class UsersController : ApiControllerBase
{
    /// <summary>
    /// Register a new student account.
    /// Assigns the "Student" realm role in Keycloak.
    /// </summary>
    [HttpPost("students")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> RegisterStudent(
        [FromBody] RegisterStudentRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(
            new RegisterStudentCommand(
                request.Username,
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password), ct);

        return result.Match<IActionResult>(
            userId => CreatedAtAction(
                nameof(RegisterStudent),
                new { userId },
                new { userId, role = "Student" }),
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }

    /// <summary>
    /// Register a new teacher account.
    /// Assigns the "Teacher" realm role in Keycloak.
    /// </summary>
    [HttpPost("teachers")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> RegisterTeacher(
        [FromBody] RegisterTeacherRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(
            new RegisterTeacherCommand(
                request.Username,
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password), ct);

        return result.Match<IActionResult>(
            userId => CreatedAtAction(
                nameof(RegisterTeacher),
                new { userId },
                new { userId, role = "Teacher" }),
            error => Problem(detail: error.Message, statusCode: (int)error.Code));
    }
}

// ── Request models ─────────────────────────────────────────────────────────────
/// <summary>
/// 
/// </summary>
/// <param name="Username"></param>
/// <param name="Email"></param>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="Password"></param>
public sealed record RegisterStudentRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password);

/// <summary>
/// 
/// </summary>
/// <param name="Username"></param>
/// <param name="Email"></param>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="Password"></param>
public sealed record RegisterTeacherRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password);
