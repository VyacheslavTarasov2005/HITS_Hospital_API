using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class TokensRepository(ApplicationDbContext dbContext) : ITokensRepository
{
    public async Task<String> Create(String token, Guid doctorId)
    {
        var newToken = new Token(token, doctorId);
        
        await dbContext.AddAsync(newToken);
        await dbContext.SaveChangesAsync();

        return newToken.AccesToken;
    }

    public async Task<Token?> Get(String accessToken)
    {
        var tokens = await dbContext.Tokens
            .AsNoTracking()
            .ToListAsync();

        foreach (var token in tokens)
        {
            if (BCrypt.Net.BCrypt.EnhancedVerify(accessToken, token.AccesToken))
            {
                return token;
            }
        }

        return null;
    }

    public async Task Delete(String accesToken)
    {
        var tokens = await dbContext.Tokens
            .AsNoTracking()
            .ToListAsync();
        
        foreach (var token in tokens)
        {
            if (BCrypt.Net.BCrypt.EnhancedVerify(accesToken, token.AccesToken))
            {
                dbContext.Tokens.Remove(token);
            }
        }
        
        await dbContext.SaveChangesAsync();
    }
}