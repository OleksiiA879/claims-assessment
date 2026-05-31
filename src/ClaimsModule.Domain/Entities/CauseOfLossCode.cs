namespace ClaimsModule.Domain.Entities;

public class CauseOfLossCode
{
    public Guid Id { get; set; }
    public Guid OrganisationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PerilCategory { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
