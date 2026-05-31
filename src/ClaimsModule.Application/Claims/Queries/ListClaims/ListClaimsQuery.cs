using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Common.Models;
using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Claims.Queries.ListClaims;

public record ListClaimsQuery(
    ClaimStatus? Status,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo,
    Guid? AssignedHandlerId,
    string? CauseOfLossCode,
    string? Search,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PaginatedList<ClaimSummaryDto>>;
