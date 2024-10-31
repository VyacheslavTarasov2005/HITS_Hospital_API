using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class CommentsRepository : ICommentsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CommentsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Create(Comment comment)
    {
        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Comment>> GetByConsultationId(Guid id)
    {
        var comments = await _dbContext.Comments
            .Where(c => c.ConsultationId == id)
            .ToListAsync();
        
        return comments;
    }

    public async Task<Comment?> GetById(Guid id)
    {
        var comment = await _dbContext.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return comment;
    }

    public async Task Update(Guid id, String content)
    {
        await _dbContext.Comments
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Content, c => content)
                .SetProperty(c => c.ModifiedDate, c => DateTime.UtcNow));
        
        await _dbContext.SaveChangesAsync();
    }
}