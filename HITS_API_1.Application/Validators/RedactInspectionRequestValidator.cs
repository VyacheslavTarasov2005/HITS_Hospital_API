using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Validators;

public class RedactInspectionRequestValidator : AbstractValidator<RedactInspectionRequest>
{
    private readonly CreateDiagnosisModelValidator _createDiagnosisModelValidator;

    public RedactInspectionRequestValidator(CreateDiagnosisModelValidator createDiagnosisModelValidator)
    {
        _createDiagnosisModelValidator = createDiagnosisModelValidator;

        RuleFor(r => r.anamnesis)
            .NotEmpty()
            .WithMessage("Необходим анамнез")
            .Length(1, 5000)
            .WithMessage("Допустимая длина анамнеза - от 1 до 5000");

        RuleFor(r => r.complaints)
            .NotEmpty()
            .WithMessage("Необходимы жалобы")
            .Length(1, 5000)
            .WithMessage("Допустимая длина жалоб - от 1 до 5000");

        RuleFor(r => r.treatment)
            .NotEmpty()
            .WithMessage("Необходимы рекомендации по лечению")
            .Length(1, 5000)
            .WithMessage("Допустимая длина рекомендаций по лечению - от 1 до 5000");

        RuleFor(r => r.conclusion)
            .NotNull()
            .WithMessage("Необходимо заключение")
            .IsInEnum()
            .WithMessage("Заключение может принимать только значения Disease, Recovery и Death");

        RuleFor(r => r.nextVisitDate)
            .Must((request, nextVisitDate) => ValidateNextVisitDateExisting(nextVisitDate, request.conclusion))
            .WithMessage("Дату следующего визита необходимо указать если заключение принимает значение Disease");

        RuleFor(r => r.deathDate)
            .Must((request, deathDate) => ValidateDeathDateExisting(deathDate, request.conclusion))
            .WithMessage("Дату смерти необходимо указать если заключение принимает значение Death");

        RuleFor(r => r.diagnoses)
            .NotEmpty()
            .WithMessage("Необходим хотя бы 1 диагноз")
            .MustAsync((diagnoses, _) => ValidateDiagnoses(diagnoses))
            .WithMessage("Некорректные диагнозы"); ;
    }

    private bool ValidateNextVisitDateExisting(DateTime? date, Conclusion conclusion)
    {
        if (conclusion != Conclusion.Disease)
        {
            return date == null;
        }

        return date != null;
    }

    private bool ValidateDeathDateExisting(DateTime? date, Conclusion conclusion)
    {
        if (conclusion == Conclusion.Death)
        {
            if (date == null)
            {
                return false;
            }

            return true;
        }

        return date == null;
    }

    private async Task<bool> ValidateDiagnoses(List<CreateDiagnosisModel> diagnoses)
    {
        foreach (var diagnosis in diagnoses)
        {
            var validationResult = await _createDiagnosisModelValidator.ValidateAsync(diagnosis);
            if (!validationResult.IsValid)
            {
                return false;
            }
        }

        return true;
    }
}