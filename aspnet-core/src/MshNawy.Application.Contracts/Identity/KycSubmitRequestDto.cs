using System;
using MediatR;

namespace MshNawy.Application.Contracts.Identity;

public class KycSubmitRequestDto : IRequest<KycStatusResponseDto>
{
    public string FullNameArabic { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string NationalIdNumber { get; set; } = string.Empty;
    public string NationalIdFrontToken { get; set; } = string.Empty;
    public string NationalIdBackToken { get; set; } = string.Empty;
    public string SelfieToken { get; set; } = string.Empty;
}
