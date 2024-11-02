using FluentValidation;
using HITS_API_1.Application.DTOs;

namespace HITS_API_1.Application.Validators;

public class GetPatientsListRequestValidator : AbstractValidator<GetPatientsListRequest>
{
    public GetPatientsListRequestValidator()
    {
        RuleFor(r => r.page)
            .GreaterThan(0)
            .When(r => r.page != null)
            .WithMessage("Номер страницы должен быть больше 0");
        
        RuleFor(r => r.size)
            .GreaterThan(0)
            .When(r => r.size != null)
            .WithMessage("Размер страницы должен быть больше 0");

        RuleFor(r => r.conclusions)
            .IsInEnum()
            .When(r => r.conclusions != null)
            .WithMessage("Допустимые значения заключения: Disease, Recovery, Death");

        RuleFor(r => r.sorting)
            .IsInEnum()
            .When(r => r.sorting != null)
            .WithMessage(
                "Допустимые значения сортировки: NameAsc, NameDesc, CreateAsc, CreateDesc, InspectionAsc, InspectionDesc");
    }
}