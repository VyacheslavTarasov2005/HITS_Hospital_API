using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class EmailMessageConfiguration : IEntityTypeConfiguration<EmailMessage>
{
    public void Configure(EntityTypeBuilder<EmailMessage> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne<Inspection>()
            .WithMany()
            .HasForeignKey(e => e.InspectionId);

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.EmailAddress)
            .IsRequired();
    }
}