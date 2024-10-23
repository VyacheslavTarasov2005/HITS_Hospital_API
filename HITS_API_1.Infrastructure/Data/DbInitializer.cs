namespace HITS_API_1.Infrastructure.Data;

public class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}