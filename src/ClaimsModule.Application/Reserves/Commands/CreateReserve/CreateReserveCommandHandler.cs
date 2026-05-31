using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reserves.Commands.CreateReserve;

public class CreateReserveCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog,
    IGlPostingJobScheduler glScheduler) : IRequestHandler<CreateReserveCommand, CreateReserveResult>
{
    public async Task<CreateReserveResult> Handle(CreateReserveCommand request, CancellationToken cancellationToken)
    {
        var claim = await context.Claims
            .Include(c => c.ReserveComponents).ThenInclude(r => r.History)
            .FirstOrDefaultAsync(c => c.Id == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        if (!claim.PolicyId.HasValue)
            throw new ValidationException("PolicyId", "Reserves cannot be set until a policy is linked.");

        var component = claim.ReserveComponents
            .FirstOrDefault(c => c.Component == request.Component);

        if (component is null)
        {
            component = new ClaimReserveComponent
            {
                Id = Guid.NewGuid(),
                OrganisationId = currentUser.OrganisationId,
                ClaimId = claim.Id,
                Component = request.Component,
                CreatedAt = DateTimeOffset.UtcNow,
                UserCreated = currentUser.UserId
            };
            context.ClaimReserveComponents.Add(component);
            claim.ReserveComponents.Add(component);
        }

        var previousBalance = component.History
            .Where(h => h.ApprovalStatus is ReserveApprovalStatus.Approved or ReserveApprovalStatus.AutoApproved)
            .Sum(h => h.Amount);

        var changeSequence = component.History.Any()
            ? component.History.Max(h => h.ChangeSequence) + 1
            : 1;

        var approvalStatus = ReserveAuthorityService.DetermineInitialApprovalStatus(Math.Abs(request.Amount));
        var newBalance = previousBalance + request.Amount;

        if (ReserveAuthorityService.ExceedsAggregateCap(
                claim.GetApprovedReserveTotal(), request.Amount, claim.ManagerOverrideForReserves))
        {
            await auditLog.LogAsync(claim.Id, "VALIDATION_ISSUE_ADDED",
                "Total reserves will exceed $10,000,000. Manager override required.", cancellationToken: cancellationToken);
        }

        var history = new ReserveHistory
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            ReserveComponentId = component.Id,
            ClaimId = claim.Id,
            TransactionType = request.TransactionType,
            Amount = request.Amount,
            PreviousBalance = previousBalance,
            NewBalance = newBalance,
            ApprovalStatus = approvalStatus,
            ChangeReason = request.ChangeReason,
            ChangeSequence = changeSequence,
            SubmittedByUserId = currentUser.UserId,
            IdempotencyKey = ReserveHistory.BuildIdempotencyKey(component.Id, changeSequence),
            CreatedAt = DateTimeOffset.UtcNow,
            UserCreated = currentUser.UserId
        };

        if (approvalStatus == ReserveApprovalStatus.AutoApproved)
        {
            history.ApprovedAt = DateTimeOffset.UtcNow;
            history.ApprovedByUserId = currentUser.UserId;
            component.CurrentAmount = newBalance;
            glScheduler.Enqueue(history.Id, claim.Id, history.IdempotencyKey);
            await auditLog.LogAsync(claim.Id, "RESERVE_AUTO_APPROVED",
                $"Reserve auto-approved: {request.Amount:C} ({request.Component}).",
                relatedEntityId: history.Id, relatedEntityType: nameof(ReserveHistory),
                cancellationToken: cancellationToken);
        }
        else
        {
            await auditLog.LogAsync(claim.Id, "RESERVE_CREATED",
                $"Reserve pending approval: {request.Amount:C} ({request.Component}).",
                relatedEntityId: history.Id, relatedEntityType: nameof(ReserveHistory),
                cancellationToken: cancellationToken);
        }

        component.History.Add(history);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateReserveResult(history.Id, component.Id, approvalStatus, history.IdempotencyKey);
    }
}
