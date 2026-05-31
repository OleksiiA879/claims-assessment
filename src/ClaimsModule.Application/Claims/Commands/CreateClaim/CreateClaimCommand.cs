using ClaimsModule.Application.Claims.DTOs;
using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.CreateClaim;

public record CreateClaimCommand(
    Guid? PolicyId,
    DateTimeOffset LossDate,
    string LossDescription,
    string CauseOfLossCode,
    string? LossLocation,
    decimal? EstimatedLossAmount,
    IReadOnlyList<CreateClaimPartyRequest> Parties,
    IReadOnlyList<CreateRiskObjectRequest> RiskObjects,
    InitialReserveRequest? InitialReserve) : IRequest<ClaimDetailDto>;
