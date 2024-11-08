using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HITS_API_1.Domain.Entities;

public class Speciality
{
    private Guid _id;
    private DateTime _createTime;
    private String _name;

    public Guid Id => _id;
    
    public DateTime CreateTime => _createTime;
    
    public String Name
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

    public Speciality(String name)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        _name = name;
    }
}