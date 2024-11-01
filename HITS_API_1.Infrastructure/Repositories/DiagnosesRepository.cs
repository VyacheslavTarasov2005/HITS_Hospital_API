using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class DiagnosesRepository : IDiagnosesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public DiagnosesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Create(Diagnosis diagnosis)
    {
        await _dbContext.Diagnoses.AddAsync(diagnosis);
        await _dbContext.SaveChangesAsync();
        
        return diagnosis.Id;
    }

    public async Task<List<Diagnosis>> GetAllByInspection(Guid inspectionId)
    {
        var diagnoses = await _dbContext.Diagnoses
            .AsNoTracking()
            .Where(d => d.InspectionId == inspectionId)
            .ToListAsync();
        
        return diagnoses;
    }
}