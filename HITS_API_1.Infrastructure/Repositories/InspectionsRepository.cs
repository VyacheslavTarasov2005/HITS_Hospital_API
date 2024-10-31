using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class InspectionsRepository : IInspectionsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InspectionsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Create(Inspection inspection)
    {
        await _dbContext.Inspections.AddAsync(inspection);
        await _dbContext.SaveChangesAsync();
        
        return inspection.Id;
    }

    public async Task<Inspection?> GetById(Guid id)
    {
        var inspection = await _dbContext.Inspections
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        
        return inspection;
    }
}