using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.Commands.RejectReserve;

public class RejectReserveCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog) : IRequestHandler<RejectReserveCommand, Unit>
{
    public async Task<Unit> Handle(RejectReserveCommand request, CancellationToken cancellationToken)
    {
        var history = await context.ReserveHistories
            .FirstOrDefaultAsync(h => h.Id == request.ReserveHistoryId && h.ClaimId == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(ReserveHistory), request.ReserveHistoryId);

        if (history.ApprovalStatus != ReserveApprovalStatus.PendingApproval)
            throw new ValidationException("ApprovalStatus", "Reserve is not pending approval.");

        if (!currentUser.IsInRole("supervisor") && !currentUser.IsInRole("manager"))
            throw new ValidationException("Role", "Your role does not have authority to reject reserves.");

        history.ApprovalStatus = ReserveApprovalStatus.Rejected;
        history.RejectedByUserId = currentUser.UserId;
        history.RejectedAt = DateTimeOffset.UtcNow;
        history.RejectionReason = request.RejectionReason;

        await auditLog.LogAsync(history.ClaimId, "RESERVE_REJECTED",
            $"Reserve rejected: {request.RejectionReason}",
            oldValue: history.Amount.ToString("F4"),
            relatedEntityId: history.Id,
            relatedEntityType: nameof(ReserveHistory),
            cancellationToken: cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
