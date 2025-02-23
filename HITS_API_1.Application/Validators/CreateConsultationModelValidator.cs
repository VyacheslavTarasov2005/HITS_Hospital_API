using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class CreateConsultationModelValidator : AbstractValidator<CreateConsultationModel>
{
    public CreateConsultationModelValidator()
    {
        RuleFor(r => r.specialityId)
            .NotEmpty()
            .WithMessage("Необходима специальность");

        RuleFor(r => r.comment)
            .NotEmpty()
            .WithMessage("Необходим комментарий")
            .Must(ValidateComment)
            .WithMessage("Допустимая длина комментария - от 1 до 1000 символов");
    }

    private bool ValidateComment(CreateInspectionCommentModel comment)
    {
        if (comment.content.Length < 1 || comment.content.Length > 1000)
        {
            return false;
        }
        
        return true;
    }
}