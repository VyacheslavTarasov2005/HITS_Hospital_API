namespace HITS_API_1.Application.Interfaces;

public interface IHasher
{
    String Hash(String password);
    bool Verify(String password, String hashedPassword);
}