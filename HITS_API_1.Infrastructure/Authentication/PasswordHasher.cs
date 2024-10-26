using HITS_API_1.Application.Interfaces;

namespace HITS_API_1.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public String HashPassword(String password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool VerifyPassword(String password, String hashedPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
}