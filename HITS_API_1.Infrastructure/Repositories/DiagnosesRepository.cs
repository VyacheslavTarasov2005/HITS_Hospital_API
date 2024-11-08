using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class DiagnosesRepository(ApplicationDbContext dbContext) : IDiagnosesRepository
{
    public async Task<Guid> Create(Diagnosis diagnosis)
    {
        await dbContext.Diagnoses.AddAsync(diagnosis);
        await dbContext.SaveChangesAsync();
        
        return diagnosis.Id;
    }

    public async Task<List<Diagnosis>> GetAllByInspection(Guid inspectionId)
    {
        var diagnoses = await dbContext.Diagnoses
            .AsNoTracking()
            .Where(d => d.InspectionId == inspectionId)
            .ToListAsync();
        
        return diagnoses;
    }

    public async Task DeleteByInspectionId(Guid inspectionId)
    {
        await dbContext.Diagnoses
            .Where(d => d.InspectionId == inspectionId)
            .ExecuteDeleteAsync();
        
        await dbContext.SaveChangesAsync();
    }
}