using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class EmailMessagesRepository : IEmailMessagesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmailMessagesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<EmailMessage?> GetByInspectionId(Guid inspectionId)
    {
        var email = _dbContext.EmailMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.InspectionId == inspectionId);
        
        return email;
    }

    public async Task Add(EmailMessage email)
    {
        await _dbContext.EmailMessages.AddAsync(email);
        await _dbContext.SaveChangesAsync();
    }
}