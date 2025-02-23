using FluentValidation;
using HITS_API_1.Application.DTOs;

namespace HITS_API_1.Application.Validators;

public class GetReportRequestValidator : AbstractValidator<GetReportRequest>
{
    public GetReportRequestValidator()
    {
        RuleFor(r => r.start)
            .NotEmpty()
            .WithMessage("Необходимо ввести начало временного интервала");

        RuleFor(r => r.end)
            .NotEmpty()
            .WithMessage("Необходимо ввести конец временного интервала");

        RuleFor(r => r)
            .Must(r => r.end >= r.start)
            .WithMessage("Некорректный временной интервал");
    }
}