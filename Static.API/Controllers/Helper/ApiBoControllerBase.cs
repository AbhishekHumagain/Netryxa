using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Static.API.Controllers.Helper;

/// <summary>
/// This Controller Base For Back Office Endpoints
/// </summary>
[ApiController]
[Route("api-bo/[controller]")]
public class ApiBoControllerBase :  ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// Mediator Service
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}