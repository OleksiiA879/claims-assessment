using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Claims.Commands.AddClaimParty;

public sealed class AddClaimPartyCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog) : IRequestHandler<AddClaimPartyCommand, PartyDto>
{
    public async Task<PartyDto> Handle(AddClaimPartyCommand request, CancellationToken cancellationToken)
    {
        var claimExists = await context.Claims.AnyAsync(c => c.Id == request.ClaimId, cancellationToken);
        if (!claimExists)
            throw new NotFoundException(nameof(Claim), request.ClaimId);

        var party = new ClaimParty
        {
            Id = Guid.NewGuid(),
            OrganisationId = currentUser.OrganisationId,
            ClaimId = request.ClaimId,
            PartyRole = request.PartyRole,
            PartyType = request.PartyType,
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            CompanyName = request.CompanyName?.Trim(),
            Email = request.Email?.Trim(),
            Phone = request.Phone?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserCreated = currentUser.UserId
        };

        context.ClaimParties.Add(party);
        await auditLog.LogAsync(request.ClaimId, "PARTY_ADDED",
            $"{party.PartyRole} party {party.DisplayName} added.",
            relatedEntityId: party.Id, relatedEntityType: nameof(ClaimParty),
            cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PartyDto(party.Id, party.PartyRole, party.PartyType, party.DisplayName, party.Email, party.Phone, party.IsActive);
    }
}
