using MediatR;

namespace ClaimsModule.Application.Claims.Commands.DeleteClaimParty;

public sealed record DeleteClaimPartyCommand(Guid ClaimId, Guid PartyId) : IRequest<Unit>;
