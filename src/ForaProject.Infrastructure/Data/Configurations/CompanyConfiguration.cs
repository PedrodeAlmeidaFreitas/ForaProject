using ForaProject.Domain.Aggregates;
using ForaProject.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForaProject.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Company aggregate.
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // Table name
        builder.ToTable("Companies");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .IsRequired()
            .ValueGeneratedNever(); // GUID generated in domain

        builder.Property(c => c.EntityName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.StandardFundableAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.SpecialFundableAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Value Object - CentralIndexKey (owned entity mapped to same table)
        builder.OwnsOne(c => c.Cik, cik =>
        {
            cik.Property(v => v.Value)
                .HasColumnName("Cik")
                .IsRequired();

            // Index on CIK for fast lookups
            cik.HasIndex(v => v.Value)
                .IsUnique()
                .HasDatabaseName("IX_Companies_Cik");
        });

        // Configure the IncomeRecords collection navigation with backing field
        builder.HasMany<ForaProject.Domain.Entities.IncomeRecord>("IncomeRecords")
            .WithOne()
            .HasForeignKey(ir => ir.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("IncomeRecords")
            .HasField("_incomeRecords")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
