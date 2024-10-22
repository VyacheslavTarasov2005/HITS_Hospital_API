using System.Runtime.InteropServices.JavaScript;

namespace HITS_API_1.Domain.Entities;

public class Doctor : Person
{
    private String _phoneNumber;
    private String _email;

    public String PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = value;
    }

    public String Email
    {
        get => _email;
        set => _email = value;
    }

    public Doctor(String name, DateTime birthday, Gender gender, String phoneNumber, String email) : base(name, birthday, gender)
    {
        _phoneNumber = phoneNumber;
        _email = email;
    }
}