using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class ClaimNumberSequenceConfiguration : IEntityTypeConfiguration<ClaimNumberSequence>
{
    public void Configure(EntityTypeBuilder<ClaimNumberSequence> builder)
    {
        builder.ToTable("ClaimNumberSequences");
        builder.HasKey(x => new { x.OrganisationId, x.Year });
        builder.Property(x => x.RowVer).IsRowVersion();
    }
}
