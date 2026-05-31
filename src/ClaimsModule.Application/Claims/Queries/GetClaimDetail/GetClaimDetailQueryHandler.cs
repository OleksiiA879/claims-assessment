using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.Queries.GetClaimDetail;

public class GetClaimDetailQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetClaimDetailQuery, ClaimDetailDto>
{
    public async Task<ClaimDetailDto> Handle(GetClaimDetailQuery request, CancellationToken cancellationToken)
    {
        var claim = await context.Claims
            .AsNoTracking()
            .Include(c => c.LossEvent)
            .Include(c => c.Parties)
            .Include(c => c.RiskObjects)
            .Include(c => c.ValidationIssues)
            .Include(c => c.ReserveComponents)
                .ThenInclude(r => r.History)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Claim), request.ClaimId);

        var reserves = claim.ReserveComponents
            .Select(c => new ReserveSummaryDto(
                c.Id,
                c.Component,
                c.History
                    .Where(h => h.ApprovalStatus is ReserveApprovalStatus.Approved or ReserveApprovalStatus.AutoApproved)
                    .Sum(h => h.Amount),
                c.History
                    .Where(h => h.ApprovalStatus == ReserveApprovalStatus.PendingApproval)
                    .Sum(h => h.Amount),
                c.History
                    .OrderByDescending(h => h.CreatedAt)
                    .Select(h => new ReserveTransactionDto(
                        h.Id, h.TransactionType, h.Amount, h.ApprovalStatus, h.PostingStatus,
                        h.ChangeReason, h.SubmittedByUserId, h.ApprovedByUserId, h.CreatedAt))
                    .ToList()))
            .ToList();

        return new ClaimDetailDto(
            claim.Id,
            claim.ClaimNumber,
            claim.Status,
            claim.PolicyNumber,
            claim.ClientName,
            claim.PolicyId,
            claim.LossEvent?.LossDate,
            claim.LossEvent?.LossDescription,
            claim.LossEvent?.LossLocation,
            claim.LossEvent?.CauseOfLossCode,
            claim.AssignedHandlerId,
            claim.ManagerOverrideForReserves,
            claim.Parties.Select(p => new PartyDto(p.Id, p.PartyRole, p.PartyType, p.DisplayName, p.Email, p.Phone, p.IsActive)).ToList(),
            claim.RiskObjects.Select(r => new RiskObjectDto(r.Id, r.AssetType, r.AssetDescription, r.DamageDescription, r.IsPrimary)).ToList(),
            reserves,
            claim.Documents.Select(d => new DocumentDto(d.Id, d.DocumentName, d.DocumentType, d.UploadedAt, d.FileSizeBytes, null)).ToList(),
            claim.ValidationIssues.Where(v => v.IsActive).Select(v => new ValidationIssueDto(v.Code, v.Message, v.Severity, v.IsAcknowledged)).ToList());
    }
}
