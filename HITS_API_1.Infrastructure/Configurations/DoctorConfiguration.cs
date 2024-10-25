using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);
        
        builder.HasOne<Speciality>()
            .WithMany()
            .HasForeignKey(d => d.Speciality);
        
        builder.Property(d => d.CreateTime)
            .IsRequired();
        
        builder.Property(d => d.Name)
            .IsRequired();
        
        builder.Property(d => d.BirthDate);
        
        builder.Property(d => d.Sex)
            .IsRequired();
        
        builder.Property(d => d.PhoneNumber);
        
        builder.Property(d => d.Email)
            .IsRequired();

        builder.Property(d => d.Password)
            .IsRequired();
    }
}