using HITS_API_1.Infrastructure.Interfaces;

namespace HITS_API_1.Infrastructure.Data;

public class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, IIcdLoader icdLoader, 
        ISpecialitiesLoader specialitiesLoader)
    {
        if (await context.Database.EnsureCreatedAsync())
        {
            await icdLoader.Load();
            await specialitiesLoader.Load();
        }
    }
}