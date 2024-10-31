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

        builder.HasMany<Token>()
            .WithOne()
            .HasForeignKey(t => t.Doctor);

        builder.HasMany<Inspection>()
            .WithOne()
            .HasForeignKey(i => i.DoctorId);

        builder.HasMany<Comment>()
            .WithOne()
            .HasForeignKey(c => c.AuthorId);
        
        builder.Property(d => d.CreateTime)
            .IsRequired();
        
        builder.Property(d => d.Name)
            .IsRequired();
        
        builder.Property(d => d.Birthday);
        
        builder.Property(d => d.Sex)
            .IsRequired();
        
        builder.Property(d => d.Phone);
        
        builder.Property(d => d.Email)
            .IsRequired();

        builder.Property(d => d.Password)
            .IsRequired();
    }
}