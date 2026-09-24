using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.RetractReserve;

public sealed record RetractReserveCommand(Guid ClaimId, Guid ReserveHistoryId, string Reason) : IRequest<Unit>;
