namespace HITS_API_1.Domain.Entities;

public class Patient
{
    private string _name;
    private DateTime _birthday;
    private Gender _gender;

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

    public Patient(string name, DateTime birthday, Gender gender)
    {
        if (name.Split(' ').Length < 3)
        {
            throw new ArgumentException("ФИО должно состоять хотя бы из 3-х слов");
        }

        if (birthday > DateTime.Now)
        {
            throw new ArgumentException("Дата рождения не может быть позже текущей даты");
        }
        
        _name = name;
        _birthday = birthday;
        _gender = gender;
    }
}