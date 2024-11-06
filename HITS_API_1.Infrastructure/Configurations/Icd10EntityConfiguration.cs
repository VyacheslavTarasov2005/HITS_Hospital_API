using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class Icd10EntityConfiguration : IEntityTypeConfiguration<Icd10Entity>
{
    public void Configure(EntityTypeBuilder<Icd10Entity> builder)
    {
        builder.ToTable("ICD10");
        
        builder.HasKey(i => i.Id);

        builder.HasOne<Icd10Entity>()
            .WithMany()
            .HasForeignKey(i => i.ParentId);
        
        builder.Property(i => i.CreateTime)
            .IsRequired();
        
        builder.Property(i => i.Code)
            .IsRequired();

        builder.Property(i => i.Name)
            .IsRequired();
    }
}