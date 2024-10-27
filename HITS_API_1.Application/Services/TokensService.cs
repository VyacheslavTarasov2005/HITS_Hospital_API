using HITS_API_1.Application.Interfaces;
using HITS_API_1.Domain;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class TokensService : ITokensService
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IHasher _hasher;

    public TokensService(ITokensRepository tokensRepository, IHasher hasher)
    {
        _tokensRepository = tokensRepository;
        _hasher = hasher;
    }

    public async Task<String> CreateToken(Guid doctorId)
    {
        String token = Guid.NewGuid().ToString();
        String hashedToken = _hasher.Hash(token);
        
        await _tokensRepository.Create(hashedToken, doctorId);
        
        return token;
    }

    public async Task<Token?> GetToken(String token)
    {
        return await _tokensRepository.Get(token);
    }

    public async Task<String> DeleteToken(String token)
    {
        String hashedToken = _hasher.Hash(token);
        return await _tokensRepository.Delete(hashedToken);
    }
}