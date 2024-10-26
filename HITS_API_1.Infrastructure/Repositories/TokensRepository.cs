using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class TokensRepository : ITokensRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public TokensRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<String> Create(Guid doctorId)
    {
        var newToken = new Token(doctorId);
        
        await _dbContext.AddAsync(newToken);
        await _dbContext.SaveChangesAsync();

        return newToken.AccesToken;
    }

    public async Task<Token?> Get(string accessToken)
    {
        var token = await _dbContext.Tokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.AccesToken == accessToken);

        return token;
    }

    public async Task<String> Delete(String token)
    {
        await _dbContext.Tokens
            .Where(t => t.AccesToken == token)
            .ExecuteDeleteAsync();
        
        return token;
    }
}