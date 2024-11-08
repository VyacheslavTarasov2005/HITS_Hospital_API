using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface ITokensRepository
{
    Task<String> Create(String token, Guid doctorId);
    Task<Token?> Get(String token);
    Task Delete(String accesToken);
    Task DeleteExpired();
}