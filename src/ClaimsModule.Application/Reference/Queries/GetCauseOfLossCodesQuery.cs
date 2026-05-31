using MediatR;
using Microsoft.EntityFrameworkCore;
using ClaimsModule.Application.Common.Interfaces;

namespace ClaimsModule.Application.Reference.Queries;

public record CauseOfLossCodeDto(string Code, string Name, string PerilCategory);
public record GetCauseOfLossCodesQuery(string? PerilCategory) : IRequest<IReadOnlyList<CauseOfLossCodeDto>>;

public class GetCauseOfLossCodesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCauseOfLossCodesQuery, IReadOnlyList<CauseOfLossCodeDto>>
{
    public async Task<IReadOnlyList<CauseOfLossCodeDto>> Handle(GetCauseOfLossCodesQuery request, CancellationToken cancellationToken)
    {
        var query = context.CauseOfLossCodes.AsNoTracking().Where(c => c.IsActive);
        if (!string.IsNullOrWhiteSpace(request.PerilCategory))
            query = query.Where(c => c.PerilCategory == request.PerilCategory);

        return await query
            .OrderBy(c => c.SortOrder)
            .Select(c => new CauseOfLossCodeDto(c.Code, c.Name, c.PerilCategory))
            .ToListAsync(cancellationToken);
    }
}
