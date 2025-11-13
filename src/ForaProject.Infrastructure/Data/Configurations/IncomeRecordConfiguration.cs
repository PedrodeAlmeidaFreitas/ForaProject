using ForaProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForaProject.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for IncomeRecord entity.
/// </summary>
public class IncomeRecordConfiguration : IEntityTypeConfiguration<IncomeRecord>
{
    public void Configure(EntityTypeBuilder<IncomeRecord> builder)
    {
        // Table name
        builder.ToTable("IncomeRecords");

        // Primary key
        builder.HasKey(ir => ir.Id);

        // Properties
        builder.Property(ir => ir.Id)
            .IsRequired()
            .ValueGeneratedNever(); // GUID generated in domain

        builder.Property(ir => ir.CompanyId)
            .IsRequired();

        builder.Property(ir => ir.Year)
            .IsRequired();

        builder.Property(ir => ir.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ir => ir.Form)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(ir => ir.Frame)
            .IsRequired(false) // Frame is optional for annual reports
            .HasMaxLength(20);

        builder.Property(ir => ir.FiledDate)
            .IsRequired();

        builder.Property(ir => ir.AccessionNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ir => ir.CreatedAt)
            .IsRequired();

        builder.Property(ir => ir.UpdatedAt);

        builder.Property(ir => ir.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(ir => ir.CompanyId)
            .HasDatabaseName("IX_IncomeRecords_CompanyId");

        builder.HasIndex(ir => ir.Year)
            .HasDatabaseName("IX_IncomeRecords_Year");

        builder.HasIndex(ir => new { ir.CompanyId, ir.Year })
            .HasDatabaseName("IX_IncomeRecords_CompanyId_Year");

        // Ignore navigation property
        builder.Ignore("_company");
    }
}
