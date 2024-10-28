using Microsoft.EntityFrameworkCore;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    public DbSet<Icd10Entity> Icd10Entities { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}