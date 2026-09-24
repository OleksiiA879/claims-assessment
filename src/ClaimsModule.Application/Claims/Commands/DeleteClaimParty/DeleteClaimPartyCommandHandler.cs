using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.Commands.DeleteClaimParty;

public sealed class DeleteClaimPartyCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog) : IRequestHandler<DeleteClaimPartyCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClaimPartyCommand request, CancellationToken cancellationToken)
    {
        var party = await context.ClaimParties
            .FirstOrDefaultAsync(p => p.Id == request.PartyId && p.ClaimId == request.ClaimId, cancellationToken)
            ?? throw new NotFoundException(nameof(ClaimParty), request.PartyId);

        party.IsDeleted = true;
        party.IsActive = false;
        party.DeletedAt = DateTimeOffset.UtcNow;
        party.UpdatedAt = DateTimeOffset.UtcNow;
        party.UserModified = currentUser.UserId;

        await auditLog.LogAsync(request.ClaimId, "PARTY_REMOVED",
            $"{party.PartyRole} party {party.DisplayName} removed.",
            relatedEntityId: party.Id, relatedEntityType: nameof(ClaimParty),
            cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
