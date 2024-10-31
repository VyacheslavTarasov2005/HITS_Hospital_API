namespace HITS_API_1.Domain.Entities;

public class Comment
{
    private Guid _id;
    private DateTime _createTime;
    private DateTime? _modifiedDate;
    private String _content;
    private Guid _authorId;
    private Guid? _parentId;
    private Guid _consultationId;

    public Guid Id => _id;
    public DateTime CreateTime => _createTime;

    public DateTime? ModifiedDate
    {
        get => _modifiedDate;
        set => _modifiedDate = value;
    }

    public String Content
    {
        get => _content;
        set => _content = value;
    }
    
    public Guid AuthorId => _authorId;
    
    public Guid? ParentId => _parentId;
    
    public Guid ConsultationId => _consultationId;

    public Comment(DateTime? modifiedDate, String content, Guid authorId, Guid? parentId, Guid consultationId)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        _modifiedDate = modifiedDate;
        _content = content;
        _authorId = authorId;
        _parentId = parentId;
        _consultationId = consultationId;
    }
}