using HITS_API_1.Application.Interfaces;

namespace HITS_API_1.Infrastructure.Authentication;

public class Hasher : IHasher
{
    public String Hash(String stringToHash)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(stringToHash);
    }

    public bool Verify(String normalString, String hashedString)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(normalString, hashedString);
    }
}