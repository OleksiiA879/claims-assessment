using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.Queries.GetClaimAudit;

public record GetClaimAuditQuery(Guid ClaimId, int PageNumber = 1, int PageSize = 50) : IRequest<PaginatedList<AuditLogDto>>;

public class GetClaimAuditQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetClaimAuditQuery, PaginatedList<AuditLogDto>>
{
    public async Task<PaginatedList<AuditLogDto>> Handle(GetClaimAuditQuery request, CancellationToken cancellationToken)
    {
        var query = context.ClaimAuditLogs.AsNoTracking().Where(a => a.ClaimId == request.ClaimId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AuditLogDto(a.Id, a.EventType, a.Description, a.CreatedAt, a.CreatedByUserId, a.OldValue, a.NewValue))
            .ToListAsync(cancellationToken);
        return PaginatedList<AuditLogDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}
