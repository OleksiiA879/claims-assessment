using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class ClaimReserveComponentConfiguration : IEntityTypeConfiguration<ClaimReserveComponent>
{
    public void Configure(EntityTypeBuilder<ClaimReserveComponent> builder)
    {
        builder.ToTable("ClaimReserveComponents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Component).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.CurrentAmount).HasPrecision(19, 4);
        builder.Property(x => x.RowVer).IsRowVersion();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ClaimDocumentConfiguration : IEntityTypeConfiguration<ClaimDocument>
{
    public void Configure(EntityTypeBuilder<ClaimDocument> builder)
    {
        builder.ToTable("ClaimDocuments");
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ClaimValidationIssueConfiguration : IEntityTypeConfiguration<ClaimValidationIssue>
{
    public void Configure(EntityTypeBuilder<ClaimValidationIssue> builder)
    {
        builder.ToTable("ClaimValidationIssues");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Severity).HasConversion<string>().HasMaxLength(20);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class CauseOfLossCodeConfiguration : IEntityTypeConfiguration<CauseOfLossCode>
{
    public void Configure(EntityTypeBuilder<CauseOfLossCode> builder)
    {
        builder.ToTable("CauseOfLossCodes");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.PolicyNumber).IsUnique();
    }
}

public class ClaimStatusTransitionConfiguration : IEntityTypeConfiguration<ClaimStatusTransition>
{
    public void Configure(EntityTypeBuilder<ClaimStatusTransition> builder)
    {
        builder.ToTable("ClaimStatusTransitions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FromStatus).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.ToStatus).HasConversion<string>().HasMaxLength(50);
    }
}

public class ClaimRiskObjectConfiguration : IEntityTypeConfiguration<ClaimRiskObject>
{
    public void Configure(EntityTypeBuilder<ClaimRiskObject> builder)
    {
        builder.ToTable("ClaimRiskObjects");
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
