using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;

namespace HITS_API_1.Domain.Entities;

public class Doctor : Person
{
    private String? _phone;
    private String _email;
    private String _password;
    private Guid _speciality;
    
    public String Phone
    {
        get => _phone;
        set => _phone = value;
    }
    
    public String Email
    {
        get => _email;
        set => _email = value;
    }

    public String Password
    {
        get => _password;
        set => _password = value;
    }

    public Guid Speciality
    {
        get => _speciality;
        set => _speciality = value;
    }

    public Doctor(String name, DateTime? birthday, Gender sex, String? phone, String email, String password, Guid speciality) : base(name, birthday, sex)
    {
        _phone = phone;
        _email = email;
        _password = password;
        _speciality = speciality;
    }
}