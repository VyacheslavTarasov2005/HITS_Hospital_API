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

    public async Task<Comment> Create(Comment comment)
    {
        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();
        
        return comment;
    }

    public async Task<List<Comment>> GetByConsultationId(Guid id)
    {
        var comments = await _dbContext.Comments
            .Where(c => c.ConsultationId == id)
            .ToListAsync();
        
        return comments;
    }
}