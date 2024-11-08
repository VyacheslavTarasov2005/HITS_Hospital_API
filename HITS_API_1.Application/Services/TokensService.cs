using HITS_API_1.Application.Interfaces;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class TokensService(ITokensRepository tokensRepository, IHasher hasher) : ITokensService
{
    public async Task<String> CreateToken(Guid doctorId)
    {
        String token = Guid.NewGuid().ToString();
        String hashedToken = hasher.Hash(token);
        
        await tokensRepository.Create(hashedToken, doctorId);
        
        return token;
    }

    public async Task<Token?> GetToken(String token)
    {
        return await tokensRepository.Get(token);
    }

    public async Task DeleteToken(String token)
    {
        await tokensRepository.Delete(token);
    }

    public async Task DeleteExpiredTokens()
    {
        await tokensRepository.DeleteExpired();
    }
}