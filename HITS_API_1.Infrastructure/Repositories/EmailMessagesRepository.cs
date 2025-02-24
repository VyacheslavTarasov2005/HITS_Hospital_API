using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class EmailMessagesRepository(ApplicationDbContext dbContext) : IEmailMessagesRepository
{
    public Task<EmailMessage?> GetByInspectionId(Guid inspectionId)
    {
        var email = dbContext.EmailMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.InspectionId == inspectionId);

        return email;
    }

    public async Task Add(EmailMessage email)
    {
        await dbContext.EmailMessages.AddAsync(email);
        await dbContext.SaveChangesAsync();
    }
}