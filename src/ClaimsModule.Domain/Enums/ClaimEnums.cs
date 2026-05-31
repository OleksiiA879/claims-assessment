namespace ClaimsModule.Domain.Enums;

public enum ClaimStatus
{
    Draft,
    Open,
    UnderInvestigation,
    PendingPayment,
    Closed,
    Reopened,
    Withdrawn,
    SlaBreached
}

public enum PartyRole
{
    Claimant,
    Insured,
    ThirdParty,
    Witness,
    Attorney
}

public enum PartyType
{
    Person,
    Company
}

public enum ReserveComponentType
{
    Indemnity,
    Expense,
    ALAE,
    SubrogationRecoverable
}

public enum ReserveApprovalStatus
{
    AutoApproved,
    PendingApproval,
    Approved,
    Rejected,
    Cancelled
}

public enum ReservePostingStatus
{
    Pending,
    Posted,
    Failed,
    Cancelled
}

public enum ReserveTransactionType
{
    Add,
    Adjust,
    Reverse
}

public enum ValidationSeverity
{
    Critical,
    Warning
}

public enum AuthorityLevel
{
    Auto,
    Supervisor,
    Manager
}
