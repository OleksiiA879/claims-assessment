using ClaimsModule.Application.Reference.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/reference")]
public class ReferenceController(IMediator mediator) : ControllerBase
{
    [HttpGet("cause-of-loss-codes")]
    public async Task<IActionResult> CauseOfLoss([FromQuery] string? perilCategory, CancellationToken ct) =>
        Ok(await mediator.Send(new GetCauseOfLossCodesQuery(perilCategory), ct));

    [HttpGet("claim-statuses")]
    public async Task<IActionResult> ClaimStatuses(CancellationToken ct) =>
        Ok(await mediator.Send(new GetClaimStatusesQuery(), ct));
}
