using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.CreateReserve;

public record CreateReserveResult(
    Guid ReserveHistoryId,
    Guid ReserveComponentId,
    ReserveApprovalStatus ApprovalStatus,
    string IdempotencyKey);

public record CreateReserveCommand(
    Guid ClaimId,
    ReserveComponentType Component,
    decimal Amount,
    string ChangeReason,
    ReserveTransactionType TransactionType = ReserveTransactionType.Add) : IRequest<CreateReserveResult>;
