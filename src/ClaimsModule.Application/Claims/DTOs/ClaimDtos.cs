using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.DTOs;

public record ClaimSummaryDto(
    Guid Id,
    string ClaimNumber,
    string? PolicyNumber,
    string? ClientName,
    DateTimeOffset? LossDate,
    string? CauseOfLossCode,
    ClaimStatus Status,
    decimal TotalReserves);

public record ClaimDetailDto(
    Guid Id,
    string ClaimNumber,
    ClaimStatus Status,
    string? PolicyNumber,
    string? ClientName,
    Guid? PolicyId,
    DateTimeOffset? LossDate,
    string? LossDescription,
    string? LossLocation,
    string? CauseOfLossCode,
    Guid? AssignedHandlerId,
    bool ManagerOverrideForReserves,
    IReadOnlyList<PartyDto> Parties,
    IReadOnlyList<RiskObjectDto> RiskObjects,
    IReadOnlyList<ReserveSummaryDto> Reserves,
    IReadOnlyList<DocumentDto> Documents,
    IReadOnlyList<ValidationIssueDto> ValidationIssues);

public record PartyDto(Guid Id, PartyRole PartyRole, PartyType PartyType, string DisplayName, string? Email, string? Phone, bool IsActive);
public record RiskObjectDto(Guid Id, string AssetType, string AssetDescription, string? DamageDescription, bool IsPrimary);
public record ReserveSummaryDto(Guid ComponentId, ReserveComponentType Component, decimal CurrentAmount, decimal PendingAmount, IReadOnlyList<ReserveTransactionDto> Transactions);
public record ReserveTransactionDto(Guid Id, ReserveTransactionType TransactionType, decimal Amount, ReserveApprovalStatus ApprovalStatus, ReservePostingStatus PostingStatus, string ChangeReason, Guid? SubmittedByUserId, Guid? ApprovedByUserId, DateTimeOffset CreatedAt);
public record DocumentDto(Guid Id, string DocumentName, string DocumentType, DateTimeOffset UploadedAt, long FileSizeBytes, string? DownloadUrl);
public record ValidationIssueDto(string Code, string Message, ValidationSeverity Severity, bool IsAcknowledged);
public record AuditLogDto(Guid Id, string EventType, string Description, DateTimeOffset CreatedAt, Guid? CreatedByUserId, string? OldValue, string? NewValue);

public record CreateClaimPartyRequest(PartyRole PartyRole, PartyType PartyType, string? FirstName, string? LastName, string? CompanyName, string? Email, string? Phone);
public record CreateRiskObjectRequest(string AssetType, string AssetDescription, string? DamageDescription, string? AssetReference, bool IsPrimary);
public record InitialReserveRequest(ReserveComponentType Component, decimal Amount, string ChangeReason);
