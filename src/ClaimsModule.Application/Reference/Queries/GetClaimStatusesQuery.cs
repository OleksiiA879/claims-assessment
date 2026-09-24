using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Reference.Queries;

public record ClaimStatusDto(string Status, IReadOnlyList<string> AllowedTransitions);
public record GetClaimStatusesQuery : IRequest<IReadOnlyList<ClaimStatusDto>>;

public class GetClaimStatusesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetClaimStatusesQuery, IReadOnlyList<ClaimStatusDto>>
{
    public async Task<IReadOnlyList<ClaimStatusDto>> Handle(GetClaimStatusesQuery request, CancellationToken cancellationToken)
    {
        var statuses = Enum.GetValues<ClaimStatus>();
        var transitions = await context.ClaimStatusTransitions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return statuses
            .Select(s => new ClaimStatusDto(
                s.ToString(),
                transitions.Where(t => t.FromStatus == s).Select(t => t.ToStatus.ToString()).ToList()))
            .ToList();
    }
}
