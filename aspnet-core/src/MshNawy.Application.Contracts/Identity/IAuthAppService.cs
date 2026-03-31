using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MshNawy.Application.Contracts.Identity;

public interface IAuthAppService : IApplicationService
{
    Task<SendOtpResponseDto> SendOtpAsync(SendOtpRequestDto request);
    Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request);
}
