using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class LossEventConfiguration : IEntityTypeConfiguration<LossEvent>
{
    public void Configure(EntityTypeBuilder<LossEvent> builder)
    {
        builder.ToTable("LossEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(x => x.EstimatedLossAmount).HasPrecision(19, 4);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ClaimPartyConfiguration : IEntityTypeConfiguration<ClaimParty>
{
    public void Configure(EntityTypeBuilder<ClaimParty> builder)
    {
        builder.ToTable("ClaimParties");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PartyRole).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.PartyType).HasConversion<string>().HasMaxLength(20);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ReserveHistoryConfiguration : IEntityTypeConfiguration<ReserveHistory>
{
    public void Configure(EntityTypeBuilder<ReserveHistory> builder)
    {
        builder.ToTable("ReserveHistory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(19, 4);
        builder.Property(x => x.PreviousBalance).HasPrecision(19, 4);
        builder.Property(x => x.NewBalance).HasPrecision(19, 4);
        builder.Property(x => x.ApprovalStatus).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.PostingStatus).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.IdempotencyKey).HasMaxLength(200);
        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ClaimAuditLogConfiguration : IEntityTypeConfiguration<ClaimAuditLog>
{
    public void Configure(EntityTypeBuilder<ClaimAuditLog> builder)
    {
        builder.ToTable("ClaimAuditLog");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(x => x.EventType).HasMaxLength(100);
    }
}
