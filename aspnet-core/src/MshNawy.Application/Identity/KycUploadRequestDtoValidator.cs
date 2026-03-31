using FluentValidation;
using MshNawy.Application.Contracts.Identity;

namespace MshNawy.Application.Identity;

public class KycUploadRequestDtoValidator : AbstractValidator<KycUploadRequestDto>
{
    private static readonly string[] AllowedFileTypes = { "NationalIdFront", "NationalIdBack", "Selfie" };

    public KycUploadRequestDtoValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty();

        RuleFor(x => x.ContentType)
            .NotEmpty();

        RuleFor(x => x.FileType)
            .NotEmpty()
            .Must(ft => Array.Exists(AllowedFileTypes, t => t == ft))
            .WithMessage("File type must be one of: NationalIdFront, NationalIdBack, Selfie.");
    }
}
