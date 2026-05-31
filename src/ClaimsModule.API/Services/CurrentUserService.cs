using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using System.Security.Claims;

namespace ClaimsModule.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId => Guid.TryParse(GetClaim(ClaimTypes.NameIdentifier), out var id)
        ? id : SeedConstants.HandlerUserId;

    public string UserName => GetClaim(ClaimTypes.Name) ?? "handler@claims.local";
    public string Role => GetClaim(ClaimTypes.Role) ?? "handler";
    public Guid OrganisationId => SeedConstants.DefaultOrganisationId;
    public Guid? CorrelationId => httpContextAccessor.HttpContext?.TraceIdentifier is { } t && Guid.TryParse(t, out var g) ? g : Guid.NewGuid();

    public bool IsInRole(string role) =>
        string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);

    private string? GetClaim(string type) =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(type);
}
