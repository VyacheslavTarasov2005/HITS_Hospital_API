using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Infrastructure.Data;

public class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, IIcd10Repository iC10Repository)
    {
        if (context.Database.EnsureCreated())
        {
            await iC10Repository.Load();
        }
    }
}