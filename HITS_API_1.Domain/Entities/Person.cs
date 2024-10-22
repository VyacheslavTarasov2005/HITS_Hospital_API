namespace HITS_API_1.Domain.Entities;

public abstract class Person
{
    private Guid _id;
    private DateTime _createTime;
    private string _name;
    private DateTime _birthday;
    private Gender _gender;

    public Guid Id => _id;
    
    public DateTime CreateTime => _createTime;

    public string Name
    {
        get => _name;
        set
        {
            if (value.Split(' ').Length < 3)
            {
                throw new ArgumentException("ФИО должно состоять хотя бы из 3-х слов");
            }
            _name = value;
        }
    }

    public DateTime Birthday
    {
        get => _birthday;
        set
        {
            if (Birthday > DateTime.Now)
            {
                throw new ArgumentException("Дата рождения не может быть позже текущей даты");
            }
            _birthday = value;
        }
    }

    public Gender Sex
    {
        get => _gender;
        set => _gender = value;
    }

    public Person(string name, DateTime birthday, Gender gender)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.Now;
        _name = name;
        _birthday = birthday;
        _gender = gender;
    }
}