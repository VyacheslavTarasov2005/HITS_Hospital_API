using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class CommentsService : ICommentsService
{
    private readonly ICommentsRepository _commentsRepository;

    public CommentsService(ICommentsRepository commentsRepository)
    {
        _commentsRepository = commentsRepository;
    }

    public async Task<Guid> CreateComment(String content, Guid parentId, Guid consultationId, Guid authorId)
    {
        var parentComment = await _commentsRepository.GetById(parentId);

        if (parentComment == null)
        {
            throw new NullReferenceException("Комментарий - родитель не найден");
        }

        if (parentComment.ConsultationId != consultationId)
        {
            throw new ArgumentException("ParentID не может ссылаться на консультацию, отличную от консультации комментария");
        }
        
        Comment comment = new Comment(null, content, authorId, parentId, consultationId);
        
        await _commentsRepository.Create(comment);
        
        return comment.Id;
    }

    public async Task RedactComment(Guid commentId, String content)
    {
        await _commentsRepository.Update(commentId, content);
    }
}