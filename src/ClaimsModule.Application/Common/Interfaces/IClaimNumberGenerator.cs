namespace ClaimsModule.Application.Common.Interfaces;

public interface IClaimNumberGenerator
{
    Task<string> GenerateNextAsync(Guid organisationId, CancellationToken cancellationToken = default);
}
