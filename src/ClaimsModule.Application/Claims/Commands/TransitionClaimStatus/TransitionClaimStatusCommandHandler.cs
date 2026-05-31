using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.Commands.TransitionClaimStatus;

public class TransitionClaimStatusCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog) : IRequestHandler<TransitionClaimStatusCommand, Unit>
{
    public async Task<Unit> Handle(TransitionClaimStatusCommand request, CancellationToken cancellationToken)
    {
        var claim = await context.Claims
            .Include(c => c.Parties)
            .Include(c => c.ReserveComponents).ThenInclude(r => r.History)
            .Include(c => c.ValidationIssues)
            .FirstOrDefaultAsync(c => c.Id == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Claim), request.ClaimId);

        if (!ClaimStatusMachine.IsTransitionAllowed(claim.Status, request.TargetStatus))
        {
            var valid = ClaimStatusMachine.GetNextStatuses(claim.Status);
            throw new ValidationException("TargetStatus",
                $"Transition from {claim.Status} to {request.TargetStatus} is not permitted. Valid: {string.Join(", ", valid)}");
        }

        if (request.TargetStatus == ClaimStatus.Open)
        {
            if (!claim.HasClaimant())
                throw new ValidationException("ClaimParties", "At least one Claimant party is required to open a claim.");
            if (claim.HasCriticalValidationIssues())
                throw new ValidationException("Validation", "Critical validation issues must be resolved before opening.");
        }

        if (request.TargetStatus == ClaimStatus.Closed)
        {
            var blocking = GetClosureBlockers(claim);
            if (blocking.Count > 0)
                throw new ValidationException("Closure", string.Join("; ", blocking));
        }

        if (request.TargetStatus == ClaimStatus.Reopened && !currentUser.IsInRole("supervisor") && !currentUser.IsInRole("manager"))
            throw new ValidationException("Role", "Supervisor role required to reopen a claim.");

        var oldStatus = claim.Status;
        claim.Status = request.TargetStatus;
        claim.UpdatedAt = DateTimeOffset.UtcNow;
        claim.UserModified = currentUser.UserId;

        if (request.TargetStatus == ClaimStatus.Closed)
        {
            claim.ClosedAt = DateTimeOffset.UtcNow;
            claim.ClosureReason = request.Reason;
        }

        if (request.TargetStatus == ClaimStatus.Reopened)
            claim.Status = ClaimStatus.Open;

        await auditLog.LogAsync(claim.Id, request.TargetStatus == ClaimStatus.Closed ? "CLAIM_CLOSED" : "STATUS_CHANGED",
            $"Status changed from {oldStatus} to {claim.Status}.",
            oldValue: oldStatus.ToString(),
            newValue: claim.Status.ToString(),
            cancellationToken: cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private static List<string> GetClosureBlockers(Domain.Entities.Claim claim)
    {
        var blockers = new List<string>();
        if (claim.ReserveComponents.SelectMany(r => r.History).Any(h => h.ApprovalStatus == ReserveApprovalStatus.PendingApproval))
            blockers.Add("Pending approval reserves exist.");
        if (claim.HasCriticalValidationIssues())
            blockers.Add("Critical validation issues remain.");
        if (!claim.HasClaimant())
            blockers.Add("No claimant party.");
        return blockers;
    }
}
