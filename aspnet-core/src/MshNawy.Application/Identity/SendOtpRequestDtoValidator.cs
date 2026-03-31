using FluentValidation;
using MshNawy.Application.Contracts.Identity;

namespace MshNawy.Application.Identity;

public class SendOtpRequestDtoValidator : AbstractValidator<SendOtpRequestDto>
{
    public SendOtpRequestDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+20\d{10}$")
            .WithMessage("Phone number must be a valid Egyptian number in +20XXXXXXXXXX format.");
    }
}
