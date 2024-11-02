using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Patient?> GetById(Guid patientId)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == patientId);
        
        return patient;
    }

    public async Task<List<Patient>> GetAllByNamePart(String name)
    {
        var patients = await _dbContext.Patients
            .AsNoTracking()
            .Where(p => p.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();
        
        return patients;
    }
}