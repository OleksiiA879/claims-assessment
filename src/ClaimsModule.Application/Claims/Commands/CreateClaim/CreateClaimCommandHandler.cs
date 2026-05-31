using AutoMapper;
using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Claims.Queries.GetClaimDetail;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.Commands.CreateClaim;

public class CreateClaimCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    IClaimNumberGenerator claimNumberGenerator,
    ICurrentUserService currentUser,
    IAuditLogService auditLog,
    IMediator mediator) : IRequestHandler<CreateClaimCommand, ClaimDetailDto>
{
    public async Task<ClaimDetailDto> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        Policy? policy = null;
        if (request.PolicyId.HasValue)
        {
            policy = await context.Policies
                .FirstOrDefaultAsync(p => p.Id == request.PolicyId.Value, cancellationToken);
        }

        var claimNumber = await claimNumberGenerator.GenerateNextAsync(currentUser.OrganisationId, cancellationToken);

        var claim = Domain.Entities.Claim.Create(
            currentUser.OrganisationId,
            claimNumber,
            request.PolicyId,
            policy?.PolicyNumber,
            policy?.ClientName,
            currentUser.UserId);

        claim.LossEvent = new LossEvent
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            ClaimId = claim.Id,
            LossDate = request.LossDate,
            LossDescription = request.LossDescription,
            LossLocation = request.LossLocation,
            CauseOfLossCode = request.CauseOfLossCode,
            EstimatedLossAmount = request.EstimatedLossAmount,
            ReportDate = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UserCreated = currentUser.UserId
        };

        claim.Parties = request.Parties.Select(p => new ClaimParty
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            ClaimId = claim.Id,
            PartyRole = p.PartyRole,
            PartyType = p.PartyType,
            FirstName = p.FirstName,
            LastName = p.LastName,
            CompanyName = p.CompanyName,
            Email = p.Email,
            Phone = p.Phone,
            CreatedAt = DateTimeOffset.UtcNow,
            UserCreated = currentUser.UserId
        }).ToList();

        claim.RiskObjects = request.RiskObjects.Select(r => new ClaimRiskObject
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            ClaimId = claim.Id,
            AssetType = r.AssetType,
            AssetDescription = r.AssetDescription,
            DamageDescription = r.DamageDescription,
            AssetReference = r.AssetReference,
            IsPrimary = r.IsPrimary,
            CreatedAt = DateTimeOffset.UtcNow,
            UserCreated = currentUser.UserId
        }).ToList();

        await ApplyValidationIssuesAsync(claim, policy, request, cancellationToken);

        context.Claims.Add(claim);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.InitialReserve is { } reserve && claim.PolicyId.HasValue)
        {
            await mediator.Send(new Reserves.Commands.CreateReserve.CreateReserveCommand(
                claim.Id, reserve.Component, reserve.Amount, reserve.ChangeReason, ReserveTransactionType.Add), cancellationToken);
        }

        return await mediator.Send(new GetClaimDetailQuery(claim.Id), cancellationToken);
    }

    private async Task ApplyValidationIssuesAsync(
        Domain.Entities.Claim claim,
        Policy? policy,
        CreateClaimCommand request,
        CancellationToken cancellationToken)
    {
        var issues = new List<ClaimValidationIssue>();

        if (!request.PolicyId.HasValue)
        {
            issues.Add(CreateIssue(claim, "NO_POLICY", "No policy linked. Policy must be associated before reserves can be set.", ValidationSeverity.Warning));
        }

        if (policy is not null)
        {
            var lossDate = DateOnly.FromDateTime(request.LossDate.UtcDateTime);
            if (lossDate < policy.EffectiveDate || lossDate > policy.ExpirationDate)
            {
                issues.Add(CreateIssue(claim, "LOSS_OUTSIDE_POLICY",
                    "Loss date is outside the policy effective period.", ValidationSeverity.Warning));
                await auditLog.LogAsync(claim.Id, "VALIDATION_ISSUE_ADDED",
                    "Loss date is outside the policy effective period.", cancellationToken: cancellationToken);
            }
        }

        var codeActive = await context.CauseOfLossCodes
            .AnyAsync(c => c.Code == request.CauseOfLossCode && c.IsActive && c.OrganisationId == currentUser.OrganisationId, cancellationToken);
        if (!codeActive)
            issues.Add(CreateIssue(claim, "INVALID_CAUSE", "Cause of loss code is not recognised or is inactive.", ValidationSeverity.Critical));

        claim.ValidationIssues = issues;
    }

    private ClaimValidationIssue CreateIssue(Domain.Entities.Claim claim, string code, string message, ValidationSeverity severity) =>
        new()
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            ClaimId = claim.Id,
            Code = code,
            Message = message,
            Severity = severity,
            CreatedAt = DateTimeOffset.UtcNow,
            UserCreated = currentUser.UserId
        };
}
