using ClaimsModule.Application.Claims.Commands.CreateClaim;
using ClaimsModule.Application.Claims.Commands.TransitionClaimStatus;
using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Claims.Queries.GetClaimAudit;
using ClaimsModule.Application.Claims.Queries.GetClaimDetail;
using ClaimsModule.Application.Claims.Queries.ListClaims;
using ClaimsModule.Application.Common.Models;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/claims")]
[Authorize]
public class ClaimsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ClaimDetailDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateClaimCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ClaimSummaryDto>>> List(
        [FromQuery] ClaimStatus? status,
        [FromQuery] DateTimeOffset? dateFrom,
        [FromQuery] DateTimeOffset? dateTo,
        [FromQuery] Guid? assignedHandlerId,
        [FromQuery] string? causeOfLossCode,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListClaimsQuery(status, dateFrom, dateTo, assignedHandlerId, causeOfLossCode, search, pageNumber, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClaimDetailDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetClaimDetailQuery(id), ct));

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> TransitionStatus(Guid id, [FromBody] TransitionStatusRequest body, CancellationToken ct)
    {
        await mediator.Send(new TransitionClaimStatusCommand(id, body.TargetStatus, body.Reason), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/audit")]
    public async Task<ActionResult<PaginatedList<AuditLogDto>>> Audit(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetClaimAuditQuery(id, pageNumber, pageSize), ct));
}

public record TransitionStatusRequest(ClaimStatus TargetStatus, string? Reason);
