using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class SpecialitiesRepository(ApplicationDbContext dbContext) : ISpecialitiesRepository
{
    public async Task<Speciality?> GetById(Guid specialityId)
    {
        Speciality? speciality = await dbContext.Specialities.FindAsync(specialityId);
        return speciality;
    }

    public async Task<List<Speciality>> GetAllByName(String name)
    {
        var specialities = await dbContext.Specialities
            .AsNoTracking()
            .Where(s => s.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();
        
        return specialities;
    }
}