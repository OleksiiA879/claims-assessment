using ClaimsModule.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Services;

public class ClaimNumberGenerator(ClaimsDbContext context) : IClaimNumberGenerator
{
    public async Task<string> GenerateNextAsync(Guid organisationId, CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await context.ClaimNumberSequences
            .FirstOrDefaultAsync(s => s.OrganisationId == organisationId && s.Year == year, cancellationToken);

        if (sequence is null)
        {
            sequence = new Domain.Entities.ClaimNumberSequence
            {
                OrganisationId = organisationId,
                Year = year,
                LastSequence = 0
            };
            context.ClaimNumberSequences.Add(sequence);
        }

        sequence.LastSequence++;
        await context.SaveChangesAsync(cancellationToken);
        return $"CLM-{year}-{sequence.LastSequence:D7}";
    }
}
