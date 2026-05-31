using ClaimsModule.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Policies.Queries;

public record PolicySearchDto(Guid PolicyId, string PolicyNumber, string ClientName, DateOnly EffectiveDate, DateOnly ExpirationDate, string Status, string CoverageTypes);
public record SearchPoliciesQuery(string Q) : IRequest<IReadOnlyList<PolicySearchDto>>;

public class SearchPoliciesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<SearchPoliciesQuery, IReadOnlyList<PolicySearchDto>>
{
    public async Task<IReadOnlyList<PolicySearchDto>> Handle(SearchPoliciesQuery request, CancellationToken cancellationToken)
    {
        var term = request.Q.Trim();
        return await context.Policies
            .AsNoTracking()
            .Where(p => p.OrganisationId == currentUser.OrganisationId &&
                        (p.PolicyNumber.Contains(term) || p.ClientName.Contains(term)))
            .Select(p => new PolicySearchDto(p.Id, p.PolicyNumber, p.ClientName, p.EffectiveDate, p.ExpirationDate, p.Status, p.CoverageTypes))
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}
