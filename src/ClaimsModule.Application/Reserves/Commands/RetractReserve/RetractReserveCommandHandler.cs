using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Reserves.Commands.CreateReserve;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.Commands.RetractReserve;

public sealed class RetractReserveCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog,
    IMediator mediator) : IRequestHandler<RetractReserveCommand, Unit>
{
    public async Task<Unit> Handle(RetractReserveCommand request, CancellationToken cancellationToken)
    {
        var history = await context.ReserveHistories
            .Include(h => h.ReserveComponent)
            .FirstOrDefaultAsync(h => h.Id == request.ReserveHistoryId && h.ClaimId == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(ReserveHistory), request.ReserveHistoryId);

        if (history.SubmittedByUserId != currentUser.UserId &&
            !currentUser.IsInRole("supervisor") && !currentUser.IsInRole("manager"))
            throw new ValidationException("Role", "Only the submitter, a supervisor, or a manager can retract a reserve.");

        if (history.ApprovalStatus == ReserveApprovalStatus.PendingApproval)
        {
            history.ApprovalStatus = ReserveApprovalStatus.Cancelled;
            history.PostingStatus = ReservePostingStatus.Cancelled;
            history.UpdatedAt = DateTimeOffset.UtcNow;
            history.UserModified = currentUser.UserId;
            await auditLog.LogAsync(request.ClaimId, "RESERVE_RETRACTED",
                $"Pending reserve retracted: {request.Reason}",
                relatedEntityId: history.Id, relatedEntityType: nameof(ReserveHistory),
                cancellationToken: cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        if (history.ApprovalStatus is not (ReserveApprovalStatus.Approved or ReserveApprovalStatus.AutoApproved))
            throw new ValidationException("ApprovalStatus", "Only pending or approved reserves can be retracted.");

        await mediator.Send(new CreateReserveCommand(
            request.ClaimId,
            history.ReserveComponent?.Component ?? throw new ValidationException("Reserve", "Reserve component is missing."),
            -history.Amount,
            $"Retraction of {history.Id}: {request.Reason}",
            ReserveTransactionType.Reverse), cancellationToken);
        return Unit.Value;
    }
}
