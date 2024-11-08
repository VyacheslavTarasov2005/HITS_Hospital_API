using HITS_API_1.Infrastructure.Data.ICD10;

namespace HITS_API_1.Infrastructure.Data;

public class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, IIcdLoader icdLoader)
    {
        if (context.Database.EnsureCreated())
        {
            await icdLoader.Load();
        }
    }
}