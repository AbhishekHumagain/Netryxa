using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Static.API.Controllers.Helper;

/// <summary>
/// The Base Class for EndUser.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// Mediator
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
