using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class InspectionsRepository(ApplicationDbContext dbContext) : IInspectionsRepository
{
    public async Task<Guid> Create(Inspection inspection)
    {
        await dbContext.Inspections.AddAsync(inspection);
        await dbContext.SaveChangesAsync();

        return inspection.Id;
    }

    public async Task<Inspection?> GetById(Guid id)
    {
        var inspection = await dbContext.Inspections
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return inspection;
    }

    public async Task<Inspection?> GetByParentInspectionId(Guid parentInspectionId)
    {
        var inspection = await dbContext.Inspections
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.PreviousInspectionId == parentInspectionId);

        return inspection;
    }

    public async Task<List<Inspection>> GetAllByPatientId(Guid patientId)
    {
        var inspections = await dbContext.Inspections
            .AsNoTracking()
            .Where(i => i.PatientId == patientId)
            .ToListAsync();

        return inspections;
    }

    public async Task Update(Guid id, String anamnesis, String complaints, String treatment, Conclusion conclusion,
        DateTime? nextVisitDate, DateTime? deathDate)
    {
        await dbContext.Inspections
            .Where(i => i.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Anamnesis, i => anamnesis)
                .SetProperty(i => i.Complaints, i => complaints)
                .SetProperty(i => i.Treatment, i => treatment)
                .SetProperty(i => i.Conclusion, i => conclusion)
                .SetProperty(i => i.NextVisitDate, i => nextVisitDate)
                .SetProperty(i => i.DeathDate, i => deathDate));

        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Inspection>> GetAll()
    {
        var inspections = await dbContext.Inspections
            .AsNoTracking()
            .ToListAsync();

        return inspections;
    }
}