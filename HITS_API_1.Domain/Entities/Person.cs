using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HITS_API_1.Domain.Entities;

public abstract class Person
{
    private Guid _id;
    private DateTime _createTime;
    private string _name;
    private DateTime? _birthDate;
    private Gender _gender;
    
    public Guid Id => _id;

    public DateTime CreateTime => _createTime;
    
    public string Name
    {
        get => _name;
        set
        {
            if (value.Split(' ').Length < 2)
            {
                throw new ArgumentException("ФИО должно состоять хотя бы из 3-х слов");
            }
            _name = value;
        }
    }
    
    public DateTime? BirthDate
    {
        get => _birthDate;
        set
        {
            if (BirthDate > DateTime.UtcNow)
            {
                throw new ArgumentException("Дата рождения не может быть позже текущей даты");
            }
            _birthDate = value;
        }
    }
    
    public Gender Sex
    {
        get => _gender;
        set => _gender = value;
    }

    public Person(string name, DateTime? birthDate, Gender gender)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        _name = name;
        _birthDate = birthDate;
        _gender = gender;
    }
}