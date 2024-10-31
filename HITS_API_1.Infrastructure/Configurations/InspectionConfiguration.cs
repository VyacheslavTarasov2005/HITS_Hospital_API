using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasMany<Diagnosis>()
            .WithOne()
            .HasForeignKey(d => d.InspectionId);
        
        builder.HasMany<Consultation>()
            .WithOne()
            .HasForeignKey(c => c.InspectionId);
        
        builder.HasOne<Inspection>()
            .WithOne()
            .HasForeignKey<Inspection>(i => i.PreviousInspectionId);
        
        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(i => i.PatientId);
        
        builder.HasOne<Doctor>()
            .WithMany()
            .HasForeignKey(i => i.DoctorId);
        
        builder.Property(i => i.CreateTime)
            .IsRequired();
        
        builder.Property(i => i.Date)
            .IsRequired();

        builder.Property(i => i.Anamnesis);

        builder.Property(i => i.Complaints);

        builder.Property(i => i.Conclusion)
            .IsRequired();

        builder.Property(i => i.NextVisitDate);

        builder.Property(i => i.DeathDate);
    }
}