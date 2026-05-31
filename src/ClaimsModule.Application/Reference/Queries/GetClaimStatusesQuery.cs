using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Services;
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
        return statuses
            .Select(s => new ClaimStatusDto(
                s.ToString(),
                ClaimStatusMachine.GetNextStatuses(s).Select(x => x.ToString()).ToList()))
            .ToList();
    }
}
