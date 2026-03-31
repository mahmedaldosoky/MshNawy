using FluentValidation;
using MshNawy.Application.Contracts.Identity.Admin;

namespace MshNawy.Application.Identity.Admin;

public class ReviewKycRequestDtoValidator : AbstractValidator<ReviewKycRequestDto>
{
    public ReviewKycRequestDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Input)
            .NotNull();

        RuleFor(x => x.Input.Decision)
            .IsInEnum();

        When(x => x.Input.Decision is KycReviewDecision.Reject or KycReviewDecision.NeedsResubmission, () =>
        {
            RuleFor(x => x.Input.Reason)
                .NotEmpty()
                .WithMessage("Reason is required when rejecting or requesting resubmission.");
        });
    }
}
