using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class Icd10Repository(ApplicationDbContext dbContext) : IIcd10Repository
{
    public async Task<List<Icd10Entity>> GetAllByNamePart(String name)
    {
        if (name.Length == 0)
        {
            var icd10Entities = await dbContext.Icd10Entities
                .AsNoTracking()
                .ToListAsync();

            return icd10Entities;
        }
        else
        {
            var icd10Entities = await dbContext.Icd10Entities
                .AsNoTracking()
                .Where(i => EF.Functions.ILike(i.Name, $"%{name}%"))
                .ToListAsync();

            return icd10Entities;
        }
    }

    public async Task<List<Icd10Entity>> GetAllByCodePart(String code)
    {
        if (code.Length == 0)
        {
            var icd10Entities = await dbContext.Icd10Entities
                .AsNoTracking()
                .ToListAsync();

            return icd10Entities;
        }
        else
        {
            var icd10Entities = await dbContext.Icd10Entities
                .AsNoTracking()
                .Where(i => EF.Functions.ILike(i.Code, $"%{code}%"))
                .ToListAsync();

            return icd10Entities;
        }
    }

    public async Task<List<Icd10Entity>> GetRoots()
    {
        var icd10Entities = await dbContext.Icd10Entities
            .AsNoTracking()
            .Where(i => i.ParentId == null)
            .ToListAsync();

        return icd10Entities;
    }

    public async Task<Icd10Entity?> GetById(Guid id)
    {
        var icd10Entity = await dbContext.Icd10Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return icd10Entity;
    }

    public async Task<Icd10Entity?> GetRootByChildId(Guid childId)
    {
        var child = await GetById(childId);

        while (child?.ParentId != null)
        {
            var parent = await GetById(child.ParentId.Value);
            child = parent;
        }

        return child;
    }
}