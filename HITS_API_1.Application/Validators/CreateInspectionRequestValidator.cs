using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class CreateInspectionRequestValidator : AbstractValidator<CreateInspectionRequest>
{
    private readonly CreateDiagnosisModelValidator _createDiagnosisModelValidator;
    private readonly CreateConsultationModelValidator _createConsultationModelValidator;
    
    public CreateInspectionRequestValidator(
        CreateDiagnosisModelValidator createDiagnosisModelValidator,
        CreateConsultationModelValidator createConsultationModelValidator)
    {
        _createDiagnosisModelValidator = createDiagnosisModelValidator;
        _createConsultationModelValidator = createConsultationModelValidator;
            
        
        RuleFor(r => r.date)
            .NotEmpty()
            .WithMessage("Необходима дата")
            .Must(date => date <= DateTime.UtcNow)
            .WithMessage("Дата осмотра не может быть позже текущей даты");

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
            .WithMessage("Дату следующего визита необходимо указать если заключение принимает значение Disease")
            .Must((request, nextVisitDate) => ValidateNextVisitDate(nextVisitDate, request.date))
            .WithMessage("Дата следующего визита не может быть раньше даты осмотра");

        RuleFor(r => r.deathDate)
            .Must((request, deathDate) => ValidateDeathDateExisting(deathDate, request.conclusion))
            .WithMessage("Дату смерти необходимо указать если заключение принимает значение Death")
            .Must(deathDate => deathDate == null || deathDate <= DateTime.UtcNow)
            .WithMessage("Дата смерти не может быть позже текущей даты");

        RuleFor(r => r.diagnoses)
            .NotEmpty()
            .WithMessage("Необходим хотя бы 1 диагноз")
            .MustAsync((diagnoses, _) => ValidateDiagnoses(diagnoses))
            .WithMessage("Некорректные диагнозы");

        RuleFor(r => r.consultations)
            .MustAsync(async (consultations, _) =>
                await ValidateConsultations(consultations))
            .WithMessage("Некорректные консультации");
    }

    private bool ValidateNextVisitDateExisting(DateTime? date, Conclusion conclusion)
    {
        if (conclusion != Conclusion.Disease)
        {
            if (date == null)
            {
                return true;
            }

            return false;
        }

        if (date == null)
        {
            return false;
        }
        
        return true;
    }

    private bool ValidateNextVisitDate(DateTime? nextDate, DateTime date)
    {
        if (nextDate == null)
        {
            return true;
        }
        
        return nextDate >= date;
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

        if (date != null)
        {
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateDiagnoses(List<CreateDiagnosisModel> diagnoses)
    {
        var mainDiagnosis = false;
        foreach (var diagnosis in diagnoses)
        {
            var validationResult = await _createDiagnosisModelValidator.ValidateAsync(diagnosis);
            if (!validationResult.IsValid)
            {
                return false;
            }

            if (diagnosis.type == DiagnosisType.Main)
            {
                if (mainDiagnosis)
                {
                    return false;
                }
                
                mainDiagnosis = true;
            }
        }

        return mainDiagnosis;
    }

    private async Task<bool> ValidateConsultations(List<CreateConsultationModel>? consultations)
    {
        if (consultations == null || consultations.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < consultations.Count; i++)
        {
            var validationResult = await _createConsultationModelValidator.ValidateAsync(consultations[i]);
            
            if (!validationResult.IsValid)
            {
                return false;
            }
            
            for (int j = 0; j < i; j++)
            {
                if (consultations[j].specialityId == consultations[i].specialityId)
                {
                    return false;
                }
            }
        }
        
        return true;
    }
}