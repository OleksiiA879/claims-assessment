using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.SetManagerReserveOverride;

public sealed record SetManagerReserveOverrideCommand(Guid ClaimId, bool Enabled, string Reason) : IRequest<Unit>;
