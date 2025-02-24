using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Exceptions;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Exceptions;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class CommentsService(
    ICommentsRepository commentsRepository,
    AddCommentRequestValidator addCommentRequestValidator,
    IDoctorsRepository doctorsRepository,
    IConsultationsRepository consultationsRepository,
    RedactCommentRequestValidator redactCommentRequestValidator,
    IInspectionsRepository inspectionsRepository)
    : ICommentsService
{
    public async Task<Guid> CreateComment(Guid consultationId, AddCommentRequest request, Guid authorId)
    {
        var validationResult = await addCommentRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var author = await doctorsRepository.GetById(authorId);
        if (author == null)
        {
            throw new NotFoundObjectException("user", "Пользователь не найден");
        }

        var consultation = await consultationsRepository.GetById(consultationId);
        if (consultation == null)
        {
            throw new NotFoundObjectException("consultation", "Консультация не найдена");
        }

        if (consultation.SpecialityId != author.Speciality)
        {
            var inspection = await inspectionsRepository.GetById(consultation.InspectionId);
            if (inspection.DoctorId != authorId)
            {
                throw new ForbiddenOperationException(
                    "Пользователь не может добавлять комментарии к этой консультации");
            }
        }

        var parentComment = await commentsRepository.GetById(request.parentId);
        if (parentComment == null)
        {
            throw new NotFoundObjectException("parentComment", "Комментарий - родитель не найден");
        }

        if (parentComment.ConsultationId != consultationId)
        {
            throw new IncorrectFieldException("parentId",
                "ParentID не может ссылаться на консультацию, отличную от консультации комментария");
        }

        Comment comment = new Comment(null, request.content, authorId, request.parentId, consultationId);
        await commentsRepository.Create(comment);

        return comment.Id;
    }

    public async Task RedactComment(Guid commentId, RedactCommentRequest request, Guid authorId)
    {
        var validationResult = await redactCommentRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var comment = await commentsRepository.GetById(commentId);
        if (comment == null)
        {
            throw new NotFoundObjectException("comment", "Комментарий не найден");
        }

        if (comment.AuthorId != authorId)
        {
            throw new ForbiddenOperationException("Пользователь может изменять только свои комментарии");
        }

        if (comment.Content == request.content)
        {
            throw new IncorrectFieldException("content", "Нельзя изменять содержимое комментария на такое же");
        }

        await commentsRepository.Update(commentId, request.content);
    }
}