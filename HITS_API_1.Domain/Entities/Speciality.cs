namespace HITS_API_1.Domain.Entities;

public class Speciality
{
    private Guid _id;
    private DateTime _createTime;
    private string _name;
    
    public Guid Id => _id;
    
    public DateTime CreateTime => _createTime;

    public string Name
    {
        get => _name;
        set
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("У специализации должно быть название");
            }
            _name = value;
        }
    }
}