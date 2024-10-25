using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HITS_API_1.Domain.Entities;

public class Speciality
{
    private DateTime _createTime;
    private string _name;
    
    public Guid Id { get; set; }
    
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

    public Speciality(string name)
    {
        Id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        _name = name;
    }
}