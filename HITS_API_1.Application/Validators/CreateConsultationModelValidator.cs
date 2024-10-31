using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class CreateConsultationModelValidator : AbstractValidator<CreateConsultationModel>
{
    private readonly ISpecialitiesRepository _specialitiesRepository;
    
    public CreateConsultationModelValidator(ISpecialitiesRepository specialitiesRepository)
    {
        _specialitiesRepository = specialitiesRepository;

        RuleFor(r => r.specialityId)
            .NotEmpty()
            .WithMessage("Необходима специальность")
            .MustAsync(async (specialityId, cancellationToken) => await ValidateSpeciality(specialityId))
            .WithMessage("Специальность не найдена");

        RuleFor(r => r.comment)
            .NotEmpty()
            .WithMessage("Необходим комментарий")
            .Must(ValidateComment)
            .WithMessage("Допустимая длина комментария - от 1 до 1000 символов");
    }

    public async Task<bool> ValidateSpeciality(Guid specialityId)
    {
        var speciality = await _specialitiesRepository.GetById(specialityId);

        if (speciality == null)
        {
            return false;
        }
        
        return true;
    }

    public bool ValidateComment(CreateInspectionCommentModel comment)
    {
        if (comment.content == null || comment.content.Length < 1 || comment.content.Length > 1000)
        {
            return false;
        }
        
        return true;
    }
}