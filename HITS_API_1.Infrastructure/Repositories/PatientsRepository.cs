using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class PatientsRepository(ApplicationDbContext dbContext) : IPatientsRepository
{
    public async Task<Guid> Create(Patient patient)
    {
        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();
        
        return patient.Id;
    }

    public async Task<Patient?> GetById(Guid patientId)
    {
        var patient = await dbContext.Patients.FirstOrDefaultAsync(p => p.Id == patientId);
        
        return patient;
    }

    public async Task<List<Patient>> GetAll()
    {
        var patients = await dbContext.Patients
            .AsNoTracking()
            .ToListAsync();
        
        return patients;
    }

    public async Task<List<Patient>> GetAllByNamePart(String name)
    {
        if (name.Length == 0)
        {
            return await GetAll();
        }

        return await dbContext.Patients
            .AsNoTracking()
            .Where(p => EF.Functions.ILike(p.Name, $"%{name}%"))
            .ToListAsync();
    }
}