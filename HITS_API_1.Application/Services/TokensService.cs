using HITS_API_1.Domain;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class TokensService : ITokensService
{
    private readonly ITokensRepository _tokensRepository;

    public TokensService(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task<String> CreateToken(Guid doctorId)
    {
        return await _tokensRepository.Create(doctorId);
    }

    public async Task<Token?> GetToken(String token)
    {
        return await _tokensRepository.Get(token);
    }

    public async Task<String> DeleteToken(String token)
    {
        return await _tokensRepository.Delete(token);
    }
}