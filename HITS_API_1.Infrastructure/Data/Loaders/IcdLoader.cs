using System.Text.Json;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Infrastructure.Data.ICD10;
using HITS_API_1.Infrastructure.Interfaces;

namespace HITS_API_1.Infrastructure.Data.Loaders;

public class IcdLoader(ApplicationDbContext dbContext) : IIcdLoader
{
    public async Task Load()
    {
        string filePath = "C:\\Users\\vt45\\RiderProjects\\HITS_API_1\\HITS_API_1.Infrastructure\\Data\\ICD10\\Icd10_json.json";

        var jsonData = await File.ReadAllTextAsync(filePath);
        var icd10Data = JsonSerializer.Deserialize<Icd10ListJsonEntity>(jsonData);

        if (icd10Data == null)
        {
            return;
        }

        var icd10Dictionary = new Dictionary<String, Icd10Entity>();

        foreach (var icd10ListObject in icd10Data.records)
        {
            Icd10Entity icd10Object = new Icd10Entity(icd10ListObject.MKB_CODE, icd10ListObject.MKB_NAME);

            icd10Dictionary[icd10ListObject.ID.ToString()] = icd10Object;
        }

        foreach (var icd10ListObject in icd10Data.records)
        {
            if (icd10ListObject.ID_PARENT != null)
            {
                var parent = icd10Dictionary[icd10ListObject.ID_PARENT];
                icd10Dictionary[icd10ListObject.ID.ToString()].ParentId = parent.Id;
            }
        }

        await dbContext.AddRangeAsync(icd10Dictionary.Values);

        await dbContext.SaveChangesAsync();
    }
}