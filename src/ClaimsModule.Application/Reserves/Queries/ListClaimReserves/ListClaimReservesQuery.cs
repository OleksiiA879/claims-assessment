using ClaimsModule.Application.Claims.DTOs;
using MediatR;

namespace ClaimsModule.Application.Reserves.Queries.ListClaimReserves;

public sealed record ListClaimReservesQuery(Guid ClaimId) : IRequest<IReadOnlyList<ReserveSummaryDto>>;
