using System.Text.Json;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using HITS_API_1.Infrastructure.Data.ICD10;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class Icd10Repository : IIcd10Repository
{
    private readonly ApplicationDbContext _dbContext;

    public Icd10Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Load()
    {
        string filePath = "C:\\Users\\vt45\\RiderProjects\\HITS_API_1\\HITS_API_1.Infrastructure\\Data\\ICD10\\Icd10_json.json";
        
        var jsonData = await File.ReadAllTextAsync(filePath);
        var icd10Data = JsonSerializer.Deserialize<Icd10ListJsonEntity>(jsonData);
        
        var icd10Dictionary = new Dictionary<String, Icd10Entity>();
        
        foreach (var Icd10ListObject in icd10Data.records)
        {
            Icd10Entity icd10Object = new Icd10Entity(Icd10ListObject.ID, Icd10ListObject.MKB_CODE, 
                Icd10ListObject.MKB_NAME, Icd10ListObject.ID_PARENT);
            
            icd10Dictionary[Icd10ListObject.ID.ToString()] = icd10Object;
        }

        foreach (var Icd10ListObject in icd10Data.records)
        {
            if (Icd10ListObject.ID_PARENT != null)
            {
                var parent = icd10Dictionary[Icd10ListObject.ID_PARENT];
                icd10Dictionary[Icd10ListObject.ID.ToString()].setParentId(parent.Id);
            }
        }
        
        var icd10List = icd10Dictionary.Values.ToList();
        
        await _dbContext.AddRangeAsync(icd10List);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Icd10Entity>> GetAllByName(String name)
    {
        var icd10Entities = await _dbContext.Icd10Entities
            .AsNoTracking()
            .Where(i => i.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();
        
        return icd10Entities;
    }
    
    public async Task<List<Icd10Entity>> GetAllByCode(String code)
    {
        var icd10Entities = await _dbContext.Icd10Entities
            .AsNoTracking()
            .Where(i => i.Code.ToLower().Contains(code.ToLower()))
            .ToListAsync();
        
        return icd10Entities;
    }
    
    public async Task<List<Icd10Entity>> GetRoots()
    {
        var icd10Entities = await _dbContext.Icd10Entities
            .AsNoTracking()
            .Where(i => i.ParentId == null)
            .ToListAsync();
        
        return icd10Entities;
    }

    public async Task<Icd10Entity?> GetById(Guid id)
    {
        var icd10Entity = await _dbContext.Icd10Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
        
        return icd10Entity;
    }

    public async Task<List<Icd10Entity>> GetAllByRoot(Guid rootId, List<Icd10Entity>? rootChildren = null)
    {
        rootChildren ??= new List<Icd10Entity>();
        
        var children = await _dbContext.Icd10Entities
            .AsNoTracking()
            .Where(i => i.ParentId == rootId)
            .ToListAsync();
        
        rootChildren.AddRange(children);

        foreach (var child in children)
        {
            await GetAllByRoot(child.Id, rootChildren);
        }
        
        return rootChildren;
    }
}