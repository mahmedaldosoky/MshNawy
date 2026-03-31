using MediatR;

namespace MshNawy.Application.Contracts.Identity;

public class SendOtpRequestDto : IRequest<SendOtpResponseDto>
{
    public string PhoneNumber { get; set; } = string.Empty;
}
