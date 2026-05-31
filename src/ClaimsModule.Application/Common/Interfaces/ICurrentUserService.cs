namespace ClaimsModule.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string UserName { get; }
    string Role { get; }
    Guid OrganisationId { get; }
    Guid? CorrelationId { get; }
    bool IsInRole(string role);
}
