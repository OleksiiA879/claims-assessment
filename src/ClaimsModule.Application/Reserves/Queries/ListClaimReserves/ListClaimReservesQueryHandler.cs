using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.Queries.ListClaimReserves;

public sealed class ListClaimReservesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ListClaimReservesQuery, IReadOnlyList<ReserveSummaryDto>>
{
    public async Task<IReadOnlyList<ReserveSummaryDto>> Handle(ListClaimReservesQuery request, CancellationToken cancellationToken)
    {
        if (!await context.Claims.AsNoTracking().AnyAsync(c => c.Id == request.ClaimId, cancellationToken))
            throw new NotFoundException(nameof(Claim), request.ClaimId);

        return await context.ClaimReserveComponents
            .AsNoTracking()
            .Where(c => c.ClaimId == request.ClaimId)
            .Select(c => new ReserveSummaryDto(
                c.Id,
                c.Component,
                c.History.Where(h => h.ApprovalStatus == ReserveApprovalStatus.Approved ||
                                      h.ApprovalStatus == ReserveApprovalStatus.AutoApproved).Sum(h => h.Amount),
                c.History.Where(h => h.ApprovalStatus == ReserveApprovalStatus.PendingApproval).Sum(h => h.Amount),
                c.History.OrderByDescending(h => h.CreatedAt)
                    .Select(h => new ReserveTransactionDto(h.Id, h.TransactionType, h.Amount,
                        h.ApprovalStatus, h.PostingStatus, h.ChangeReason, h.SubmittedByUserId,
                        h.ApprovedByUserId, h.CreatedAt))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}
