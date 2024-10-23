using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
{
    public void Configure(EntityTypeBuilder<Speciality> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasMany<Doctor>()
            .WithOne()
            .HasForeignKey(d => d.SpecialityId);
        
        builder.Property(s => s.CreateTime).IsRequired();
        builder.Property(s => s.Name).IsRequired();
    }
}