using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;

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
}