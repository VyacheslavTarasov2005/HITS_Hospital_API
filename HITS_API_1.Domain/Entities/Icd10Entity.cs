namespace HITS_API_1.Domain.Entities;

public class Icd10Entity
{
    private Guid _id;
    private Guid? _parentId;
    private DateTime _createTime;
    private int _icdId;
    private String _code;
    private String? _name;
    private String? _parentIcdId;

    public Guid Id => _id;
    
    public Guid? ParentId => _parentId;
    
    public DateTime CreateTime => _createTime;
    
    public int IcdId => _icdId;

    public String Code => _code;

    public String? Name => _name;
    
    public String? ParentIcdId => _parentIcdId;

    public Icd10Entity(int icdId, String code, String name, String? parentIcdId)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        _icdId = icdId;
        _code = code;
        _name = name;
        _parentIcdId = parentIcdId;
    }

    public void setParentId(Guid? parentId)
    {
        if (parentId != null)
        {
            _parentId = parentId;
        }
    }
}