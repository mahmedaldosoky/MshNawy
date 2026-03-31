using FluentValidation;
using MshNawy.Application.Contracts.Identity;

namespace MshNawy.Application.Identity;

public class VerifyOtpRequestDtoValidator : AbstractValidator<VerifyOtpRequestDto>
{
    public VerifyOtpRequestDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+20\d{10}$")
            .WithMessage("Phone number must be a valid Egyptian number in +20XXXXXXXXXX format.");

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .Length(6)
            .Matches(@"^\d{6}$")
            .WithMessage("OTP code must be exactly 6 digits.");
    }
}
