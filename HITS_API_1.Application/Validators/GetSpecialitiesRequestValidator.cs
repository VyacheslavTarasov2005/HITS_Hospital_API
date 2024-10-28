using FluentValidation;
using HITS_API_1.Application.DTOs;

namespace HITS_API_1.Application.Validators;

public class GetSpecialitiesRequestValidator : AbstractValidator<GetSpecialitiesRequest>
{
    public GetSpecialitiesRequestValidator()
    {
        RuleFor(r => r.page)
            .NotEmpty()
            .WithMessage("Необходим номер страницы")
            .GreaterThan(0)
            .WithMessage("Номер страницы должен быть больше 0");
        
        RuleFor(r => r.size)
            .NotEmpty()
            .WithMessage("Необходим размер страницы")
            .GreaterThan(0)
            .WithMessage("Размер страницы должен быть больше 0");
    }
}