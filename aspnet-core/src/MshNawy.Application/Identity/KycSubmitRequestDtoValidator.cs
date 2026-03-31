using FluentValidation;
using MshNawy.Application.Contracts.Identity;

namespace MshNawy.Application.Identity;

public class KycSubmitRequestDtoValidator : AbstractValidator<KycSubmitRequestDto>
{
    public KycSubmitRequestDtoValidator()
    {
        RuleFor(x => x.FullNameArabic)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty();

        RuleFor(x => x.NationalIdNumber)
            .NotEmpty()
            .Length(14)
            .Matches(@"^\d{14}$")
            .WithMessage("National ID must be exactly 14 digits.");

        RuleFor(x => x.NationalIdFrontToken)
            .NotEmpty();

        RuleFor(x => x.NationalIdBackToken)
            .NotEmpty();

        RuleFor(x => x.SelfieToken)
            .NotEmpty();
    }
}
