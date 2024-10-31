using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne<Inspection>()
            .WithMany()
            .HasForeignKey(c => c.InspectionId);
        
        builder.HasOne<Speciality>()
            .WithMany()
            .HasForeignKey(c => c.SpecialityId);

        builder.HasMany<Comment>()
            .WithOne()
            .HasForeignKey(c => c.ConsultationId);
        
        builder.Property(c => c.CreateTime)
            .IsRequired();
    }
}