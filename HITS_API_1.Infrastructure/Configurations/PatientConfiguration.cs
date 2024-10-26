using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.CreateTime)
            .IsRequired();
        
        builder.Property(d => d.Name)
            .IsRequired();
        
        builder.Property(d => d.Birthday);
        
        builder.Property(d => d.Sex)
            .IsRequired();
    }
}