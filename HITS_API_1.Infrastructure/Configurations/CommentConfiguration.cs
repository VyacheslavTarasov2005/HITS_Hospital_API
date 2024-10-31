using HITS_API_1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HITS_API_1.Infrastructure.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne<Consultation>()
            .WithMany()
            .HasForeignKey(c => c.ConsultationId);

        builder.HasOne<Doctor>()
            .WithOne()
            .HasForeignKey<Comment>(c => c.AuthorId);
        
        builder.HasOne<Comment>()
            .WithOne()
            .HasForeignKey<Comment>(c => c.ParentId);

        builder.Property(c => c.CreateTime)
            .IsRequired();
        
        builder.Property(c => c.Content)
            .IsRequired();
        
        builder.Property(c => c.ModifiedDate);
    }
}