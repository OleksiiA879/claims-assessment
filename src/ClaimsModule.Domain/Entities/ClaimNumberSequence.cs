namespace ClaimsModule.Domain.Entities;

public class ClaimNumberSequence
{
    public Guid OrganisationId { get; set; }
    public int Year { get; set; }
    public int LastSequence { get; set; }
    public byte[] RowVer { get; set; } = [];
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
