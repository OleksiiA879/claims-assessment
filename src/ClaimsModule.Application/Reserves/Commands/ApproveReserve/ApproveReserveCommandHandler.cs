using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.Commands.ApproveReserve;

public class ApproveReserveCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog,
    IGlPostingJobScheduler glScheduler) : IRequestHandler<ApproveReserveCommand, Unit>
{
    public async Task<Unit> Handle(ApproveReserveCommand request, CancellationToken cancellationToken)
    {
        var history = await context.ReserveHistories
            .Include(h => h.ReserveComponent)
            .FirstOrDefaultAsync(h => h.Id == request.ReserveHistoryId && h.ClaimId == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(ReserveHistory), request.ReserveHistoryId);

        if (history.ApprovalStatus != ReserveApprovalStatus.PendingApproval)
            throw new ValidationException("ApprovalStatus", "Reserve is not pending approval.");

        if (history.SubmittedByUserId == currentUser.UserId)
            throw new ValidationException("Approver", "Self-approval is not permitted.");

        var required = ReserveAuthorityService.GetRequiredAuthority(Math.Abs(history.Amount));
        if (!ReserveAuthorityService.CanApprove(currentUser.Role, Math.Abs(history.Amount), required))
            throw new ValidationException("Role", "Your role does not have authority to approve this reserve amount.");

        history.ApprovalStatus = ReserveApprovalStatus.Approved;
        history.ApprovedByUserId = currentUser.UserId;
        history.ApprovedAt = DateTimeOffset.UtcNow;
        history.UpdatedAt = DateTimeOffset.UtcNow;

        if (history.ReserveComponent is not null)
            history.ReserveComponent.CurrentAmount = history.NewBalance;

        glScheduler.Enqueue(history.Id, history.ClaimId, history.IdempotencyKey);

        await auditLog.LogAsync(history.ClaimId, "RESERVE_APPROVED",
            $"Reserve approved: {history.Amount:C}.",
            relatedEntityId: history.Id, relatedEntityType: nameof(ReserveHistory),
            cancellationToken: cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
