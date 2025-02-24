using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class ConsultationsRepository(ApplicationDbContext dbContext) : IConsultationsRepository
{
    public async Task<Guid> Create(Consultation consultation)
    {
        await dbContext.Consultations.AddAsync(consultation);
        await dbContext.SaveChangesAsync();

        return consultation.Id;
    }

    public async Task<Consultation?> GetById(Guid id)
    {
        var consultation = await dbContext.Consultations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return consultation;
    }

    public async Task<List<Consultation>> GetAllByInspectionId(Guid inspectionId)
    {
        var consultations = await dbContext.Consultations
            .AsNoTracking()
            .Where(c => c.InspectionId == inspectionId)
            .ToListAsync();

        return consultations;
    }

    public async Task<List<Consultation>> GetAll()
    {
        var consultations = await dbContext.Consultations
            .AsNoTracking()
            .ToListAsync();

        return consultations;
    }
}