using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.HasKey(t => t.AccesToken);
        
        builder.Property(t => t.ExpiryDate)
            .IsRequired();

        builder.HasOne<Doctor>()
            .WithMany()
            .HasForeignKey(t => t.Doctor);
    }
}