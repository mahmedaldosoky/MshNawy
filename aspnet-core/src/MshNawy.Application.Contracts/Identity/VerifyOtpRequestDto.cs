using MediatR;

namespace MshNawy.Application.Contracts.Identity;

public class VerifyOtpRequestDto : IRequest<AuthResponseDto>
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}
