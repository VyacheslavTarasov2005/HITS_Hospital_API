using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface ITokensService
{
    Task<String> CreateToken(Guid doctorId);
    Task<Token?> GetToken(String token);
    Task DeleteToken(String token);
    Task DeleteExpiredTokens();
}