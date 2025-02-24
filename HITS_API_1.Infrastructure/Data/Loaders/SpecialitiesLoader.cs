using HITS_API_1.Domain.Entities;
using HITS_API_1.Infrastructure.Interfaces;

namespace HITS_API_1.Infrastructure.Data.Loaders;

public class SpecialitiesLoader(ApplicationDbContext dbContext) : ISpecialitiesLoader
{
    public async Task Load()
    {
        var specialities = new List<Speciality>
        {
            new Speciality("Терапевт"),
            new Speciality("Педиатор"),
            new Speciality("Хирург")
        };
        
        await dbContext.Specialities.AddRangeAsync(specialities);
        await dbContext.SaveChangesAsync();
    }
}