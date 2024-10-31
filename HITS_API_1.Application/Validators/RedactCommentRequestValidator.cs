using FluentValidation;
using HITS_API_1.Application.DTOs;

namespace HITS_API_1.Application.Validators;

public class RedactCommentRequestValidator : AbstractValidator<RedactCommentRequest>
{
    public RedactCommentRequestValidator()
    {
        RuleFor(r => r.content)
            .NotEmpty()
            .WithMessage("Комментарий не может быть пустым")
            .Length(1, 1000)
            .WithMessage("Допустимая длина комментария - от 1 до 1000");
    }
}