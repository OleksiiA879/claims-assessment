using ClaimsModule.Domain.Common;

namespace ClaimsModule.Domain.Entities;

public class ClaimDocument : AuditableEntity
{
    public Guid ClaimId { get; set; }
    public string DocumentType { get; set; } = "Other";
    public string DocumentName { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
    public Guid? UploadedByUserId { get; set; }
    public string? Notes { get; set; }

    public Claim? Claim { get; set; }
}
