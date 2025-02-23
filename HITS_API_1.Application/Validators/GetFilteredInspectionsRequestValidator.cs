using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class GetFilteredInspectionsRequestValidator : AbstractValidator<GetFilteredInspectionsRequest>
{
    public GetFilteredInspectionsRequestValidator()
    {
        RuleFor(r => r.page)
            .GreaterThan(0)
            .When(r => r.page != null)
            .WithMessage("Номер страницы должен быть больше 0");
        
        RuleFor(r => r.size)
            .GreaterThan(0)
            .When(r => r.size != null)
            .WithMessage("Размер страницы должен быть больше 0");
    }
}