namespace HITS_API_1.Application.Interfaces;

public interface IPasswordHasher
{
    String HashPassword(String password);
    bool VerifyPassword(String password, String hashedPassword);
}