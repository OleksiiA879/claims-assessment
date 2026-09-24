using ClaimsModule.Application.Reserves.Commands.ApproveReserve;
using ClaimsModule.Application.Reserves.Commands.CreateReserve;
using ClaimsModule.Application.Reserves.Commands.RejectReserve;
using ClaimsModule.Application.Reserves.Commands.RetractReserve;
using ClaimsModule.Application.Reserves.Commands.SetManagerReserveOverride;
using ClaimsModule.Application.Reserves.Queries.ListClaimReserves;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/claims/{claimId:guid}/reserves")]
[Authorize]
public class ReservesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid claimId, CancellationToken ct) =>
        Ok(await mediator.Send(new ListClaimReservesQuery(claimId), ct));

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

    [HttpPost("{reserveHistoryId:guid}/retract")]
    public async Task<IActionResult> Retract(Guid claimId, Guid reserveHistoryId, [FromBody] RetractReserveRequest body, CancellationToken ct)
    {
        await mediator.Send(new RetractReserveCommand(claimId, reserveHistoryId, body.Reason), ct);
        return NoContent();
    }

    [HttpPost("manager-override")]
    public async Task<IActionResult> SetManagerOverride(Guid claimId, [FromBody] ManagerOverrideRequest body, CancellationToken ct)
    {
        await mediator.Send(new SetManagerReserveOverrideCommand(claimId, body.Enabled, body.Reason), ct);
        return NoContent();
    }
}

public record CreateReserveRequest(Domain.Enums.ReserveComponentType Component, decimal Amount, string ChangeReason, Domain.Enums.ReserveTransactionType TransactionType = Domain.Enums.ReserveTransactionType.Add);
public record RejectReserveRequest(string RejectionReason);
public record RetractReserveRequest(string Reason);
public record ManagerOverrideRequest(bool Enabled, string Reason);
