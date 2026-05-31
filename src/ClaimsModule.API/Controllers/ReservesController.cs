using ClaimsModule.Application.Reserves.Commands.ApproveReserve;
using ClaimsModule.Application.Reserves.Commands.CreateReserve;
using ClaimsModule.Application.Reserves.Commands.RejectReserve;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/claims/{claimId:guid}/reserves")]
[Authorize]
public class ReservesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Guid claimId, [FromBody] CreateReserveRequest body, CancellationToken ct) =>
        Created(string.Empty, await mediator.Send(new CreateReserveCommand(claimId, body.Component, body.Amount, body.ChangeReason, body.TransactionType), ct));

    [HttpPost("{reserveHistoryId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid claimId, Guid reserveHistoryId, CancellationToken ct)
    {
        await mediator.Send(new ApproveReserveCommand(claimId, reserveHistoryId), ct);
        return NoContent();
    }

    [HttpPost("{reserveHistoryId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid claimId, Guid reserveHistoryId, [FromBody] RejectReserveRequest body, CancellationToken ct)
    {
        await mediator.Send(new RejectReserveCommand(claimId, reserveHistoryId, body.RejectionReason), ct);
        return NoContent();
    }
}

public record CreateReserveRequest(Domain.Enums.ReserveComponentType Component, decimal Amount, string ChangeReason, Domain.Enums.ReserveTransactionType TransactionType = Domain.Enums.ReserveTransactionType.Add);
public record RejectReserveRequest(string RejectionReason);
