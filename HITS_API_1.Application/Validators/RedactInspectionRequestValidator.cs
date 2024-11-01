using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class RedactInspectionRequestValidator : AbstractValidator<RedactInspectionRequest>
{
    private readonly IInspectionsRepository _inspectionsRepository;
    private readonly CreateDiagnosisModelValidator _createDiagnosisModelValidator;
    
    public RedactInspectionRequestValidator(
        IInspectionsRepository inspectionsRepository,
        CreateDiagnosisModelValidator createDiagnosisModelValidator)
    {
        _inspectionsRepository = inspectionsRepository;
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
            .MustAsync((diagnoses, CancellationToken) => ValidateDiagnoses(diagnoses))
            .WithMessage("Некорректные диагнозы");
    }
    
    private bool ValidateNextVisitDateExisting(DateTime? date, Conclusion conclusion)
    {
        if (conclusion == Conclusion.Death || conclusion == Conclusion.Recovery)
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

    private async Task<bool> ValidateDeathDate(DateTime? deathDate, DateTime date, Guid? previousInspectionId)
    {
        if (deathDate == null)
        {
            return true;
        }
        
        if (previousInspectionId != null)
        {
            var previousInspection = await _inspectionsRepository.GetById(previousInspectionId.Value);

            if (deathDate < previousInspection.Date)
            {
                return false;
            }
        }
        
        return deathDate <= date;
    }
    
    private async Task<bool> ValidateDiagnoses(List<CreateDiagnosisModel> diagnoses)
    {
        bool mainDiagnosis = false;
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
}