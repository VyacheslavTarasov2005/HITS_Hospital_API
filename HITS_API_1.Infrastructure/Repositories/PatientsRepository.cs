using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;

namespace HITS_API_1.Infrastructure.Repositories;

public class PatientsRepository : IPatientsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PatientsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Create(Patient patient)
    {
        await _dbContext.Patients.AddAsync(patient);
        await _dbContext.SaveChangesAsync();
        
        return patient.Id;
    }
}