using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.AddClaimParty;

public sealed record AddClaimPartyCommand(
    Guid ClaimId,
    PartyRole PartyRole,
    PartyType PartyType,
    string? FirstName,
    string? LastName,
    string? CompanyName,
    string? Email,
    string? Phone) : IRequest<PartyDto>;
