using FluentValidation;
using MshNawy.Application.Contracts.Identity.Admin;

namespace MshNawy.Application.Identity.Admin;

public class MoveToUnderReviewRequestDtoValidator : AbstractValidator<MoveToUnderReviewRequestDto>
{
    public MoveToUnderReviewRequestDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
