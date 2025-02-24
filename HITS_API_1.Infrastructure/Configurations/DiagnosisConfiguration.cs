using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class DiagnosisConfiguration : IEntityTypeConfiguration<Diagnosis>
{
    public void Configure(EntityTypeBuilder<Diagnosis> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasOne<Inspection>()
            .WithMany()
            .HasForeignKey(d => d.InspectionId);

        builder.HasOne<Icd10Entity>()
            .WithMany()
            .HasForeignKey(d => d.Icd10Id);

        builder.Property(d => d.CreateTime)
            .IsRequired();

        builder.Property(d => d.Description);

        builder.Property(d => d.Type)
            .IsRequired();
    }
}