namespace HITS_API_1.Domain.Entities;

public class Token
{
    private readonly String _accesToken;
    private readonly DateTime _expiryDate;
    private readonly Guid _doctor;

    public String AccesToken => _accesToken;

    public DateTime ExpiryDate => _expiryDate;

    public Guid Doctor => _doctor;

    public Token(String accesToken, Guid doctor)
    {
        _accesToken = accesToken;
        _expiryDate = DateTime.UtcNow.AddHours(1);
        _doctor = doctor;
    }
}