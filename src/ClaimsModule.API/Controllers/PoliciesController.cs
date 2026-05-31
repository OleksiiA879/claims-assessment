using ClaimsModule.Application.Policies.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/policies")]
public class PoliciesController(IMediator mediator) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct) =>
        Ok(await mediator.Send(new SearchPoliciesQuery(q ?? string.Empty), ct));
}
