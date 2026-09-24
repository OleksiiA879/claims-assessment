using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Policies.Queries;

public sealed record PolicyCoverageDto(
    Guid PolicyId,
    string PolicyNumber,
    DateOnly EffectiveDate,
    DateOnly ExpirationDate,
    string Status,
    IReadOnlyList<string> CoverageTypes);

public sealed record GetPolicyCoverageQuery(Guid PolicyId) : IRequest<PolicyCoverageDto>;

public sealed class GetPolicyCoverageQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPolicyCoverageQuery, PolicyCoverageDto>
{
    public async Task<PolicyCoverageDto> Handle(GetPolicyCoverageQuery request, CancellationToken cancellationToken)
    {
        var policy = await context.Policies.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PolicyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Policy), request.PolicyId);

        return new PolicyCoverageDto(policy.Id, policy.PolicyNumber, policy.EffectiveDate, policy.ExpirationDate,
            policy.Status, policy.CoverageTypes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }
}
