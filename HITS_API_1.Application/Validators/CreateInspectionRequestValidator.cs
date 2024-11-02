using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class CreateInspectionRequestValidator : AbstractValidator<CreateInspectionRequest>
{
    private readonly IInspectionsRepository _inspectionsRepository;
    private readonly CreateDiagnosisModelValidator _createDiagnosisModelValidator;
    private readonly CreateConsultationModelValidator _createConsultationModelValidator;
    
    public CreateInspectionRequestValidator(
        IInspectionsRepository inspectionsRepository,
        CreateDiagnosisModelValidator createDiagnosisModelValidator,
        CreateConsultationModelValidator createConsultationModelValidator)
    {
        _inspectionsRepository = inspectionsRepository;
        _createDiagnosisModelValidator = createDiagnosisModelValidator;
        _createConsultationModelValidator = createConsultationModelValidator;
        
        RuleFor(r => r.previousInspectionId)
            .MustAsync(async (previousInspectionId, CancellationToken) => 
                await ValidatePreviousInspection(previousInspectionId))
            .WithMessage("Некорректное значение предыдущего осмотра");
            
        
        RuleFor(r => r.date)
            .NotEmpty()
            .WithMessage("Необходима дата")
            .Must(ValidateDate)
            .WithMessage("Дата осмотра не может быть позже текущей даты")
            .MustAsync(async (request, date, CancellationToken) => await ValidateByPreviousDate(date, 
                request.previousInspectionId))
            .WithMessage("Дата осмотра не может быть раньше даты предыдущего осмотра");

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
            .MustAsync(async (request, deathDate, CancellationToken) => await ValidateDeathDate(deathDate,
                request.date, request.previousInspectionId))
            .WithMessage("Дата смерти не может быть раньше предыдущего осмотра или позже текущего");

        RuleFor(r => r.diagnoses)
            .NotEmpty()
            .WithMessage("Необходим хотя бы 1 диагноз")
            .MustAsync((diagnoses, CancellationToken) => ValidateDiagnoses(diagnoses))
            .WithMessage("Некорректные диагнозы");

        RuleFor(r => r.consultations)
            .MustAsync(async (consultations, CancellationToken) =>
                await ValidateConsultations(consultations))
            .WithMessage("Некорректные консультации");
    }
    
    private bool ValidateDate(DateTime date)
    {
        return date <= DateTime.UtcNow;
    }
    
    private async Task<bool> ValidateByPreviousDate(DateTime date, Guid? previousInspectionId)
    {
        if (date > DateTime.UtcNow)
        {
            return false;
        }

        if (previousInspectionId == null)
        {
            return true;
        }
        
        var previousInspection = await _inspectionsRepository.GetById(previousInspectionId.Value);
        
        if (previousInspection != null)
        {
            return date >= previousInspection.Date;
        }

        return true;
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

    private async Task<bool> ValidatePreviousInspection(Guid? previousInspectionId)
    {
        if (previousInspectionId == null)
        {
            return true;
        }
        
        var previousInspection = await _inspectionsRepository.GetById(previousInspectionId.Value);

        if (previousInspection == null)
        {
            return false;
        }
        
        var childInspection = await _inspectionsRepository.GetByParentInspectionId(previousInspection.Id);

        if (childInspection != null)
        {
            return false;
        }

        if (previousInspection.Conclusion == Conclusion.Death)
        {
            return false;
        }
        
        return true;
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