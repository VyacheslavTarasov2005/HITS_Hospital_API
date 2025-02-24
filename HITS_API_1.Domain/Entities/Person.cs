namespace HITS_API_1.Domain.Entities;

public abstract class Person
{
    private Guid _id;
    private DateTime _createTime;
    private string _name;
    private DateTime? _birthday;
    private Gender _gender;

    public Guid Id => _id;

    public DateTime CreateTime => _createTime;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public DateTime? Birthday
    {
        get => _birthday;
        set => _birthday = value;
    }

    public Gender Sex
    {
        get => _gender;
        set => _gender = value;
    }

    public Person(string name, DateTime? birthday, Gender gender)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        Name = name;
        Birthday = birthday;
        Sex = gender;
    }
}