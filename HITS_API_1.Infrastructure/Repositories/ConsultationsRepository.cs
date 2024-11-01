using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class ConsultationsRepository : IConsultationsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ConsultationsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Create(Consultation consultation)
    {
        await _dbContext.Consultations.AddAsync(consultation);
        await _dbContext.SaveChangesAsync();

        return consultation.Id;
    }

    public async Task<Consultation?> GetById(Guid id)
    {
        var consultation = await _dbContext.Consultations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        
        return consultation;
    }

    public async Task<List<Consultation>> GetAllByInspectionId(Guid inspectionId)
    {
        var consultations = await _dbContext.Consultations
            .AsNoTracking()
            .Where(c => c.InspectionId == inspectionId)
            .ToListAsync();
        
        return consultations;
    }
}