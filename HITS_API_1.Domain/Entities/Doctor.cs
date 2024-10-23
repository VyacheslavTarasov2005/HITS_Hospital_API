using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;

namespace HITS_API_1.Domain.Entities;

public class Doctor : Person
{
    private String _phoneNumber;
    private String _email;
    private Guid _specialityId;
    
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

    public Guid SpecialityId
    {
        get => _specialityId;
        set => _specialityId = value;
    }

    public Doctor(String name, DateTime birthDate, Gender gender, String phoneNumber, String email, Guid specialityId) : base(name, birthDate, gender)
    {
        _phoneNumber = phoneNumber;
        _email = email;
        _specialityId = specialityId;
    }
}