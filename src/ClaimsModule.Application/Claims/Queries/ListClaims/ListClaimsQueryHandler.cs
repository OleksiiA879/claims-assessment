using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Common.Models;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.Queries.ListClaims;

public class ListClaimsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ListClaimsQuery, PaginatedList<ClaimSummaryDto>>
{
    public async Task<PaginatedList<ClaimSummaryDto>> Handle(ListClaimsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Claims
            .AsNoTracking()
            .Include(c => c.LossEvent)
            .Include(c => c.ReserveComponents)
                .ThenInclude(r => r.History)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);

        if (request.DateFrom.HasValue)
            query = query.Where(c => c.LossEvent != null && c.LossEvent.LossDate >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(c => c.LossEvent != null && c.LossEvent.LossDate <= request.DateTo.Value);

        if (request.AssignedHandlerId.HasValue)
            query = query.Where(c => c.AssignedHandlerId == request.AssignedHandlerId.Value);

        if (!string.IsNullOrWhiteSpace(request.CauseOfLossCode))
            query = query.Where(c => c.LossEvent != null && c.LossEvent.CauseOfLossCode == request.CauseOfLossCode);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(c =>
                c.ClaimNumber.Contains(term) ||
                (c.ClientName != null && c.ClientName.Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ClaimSummaryDto(
                c.Id,
                c.ClaimNumber,
                c.PolicyNumber,
                c.ClientName,
                c.LossEvent != null ? c.LossEvent.LossDate : null,
                c.LossEvent != null ? c.LossEvent.CauseOfLossCode : null,
                c.Status,
                c.ReserveComponents
                    .SelectMany(r => r.History)
                    .Where(h => h.ApprovalStatus == ReserveApprovalStatus.Approved || h.ApprovalStatus == ReserveApprovalStatus.AutoApproved)
                    .Sum(h => h.Amount)))
            .ToListAsync(cancellationToken);

        return PaginatedList<ClaimSummaryDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}
