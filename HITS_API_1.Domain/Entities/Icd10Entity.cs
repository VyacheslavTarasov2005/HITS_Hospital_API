namespace HITS_API_1.Domain.Entities;

public class Icd10Entity
{
    private Guid _id;
    private Guid? _parentId;
    private DateTime _createTime;
    private String _code;
    private String _name;

    public Guid Id => _id;

    public Guid? ParentId
    {
        get => _parentId;
        set => _parentId ??= value;
    }
    
    public DateTime CreateTime => _createTime;

    public String Code => _code;

    public String Name => _name;

    public Icd10Entity(String code, String name)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        _code = code;
        _name = name;
    }
}