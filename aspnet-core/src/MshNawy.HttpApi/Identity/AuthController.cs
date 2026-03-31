using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using MshNawy.Application.Contracts.Identity;

namespace MshNawy.HttpApi.Identity;

[Route("api/app/auth")]
public class AuthController : AbpController
{
    private readonly IAuthAppService authAppService;

    public AuthController(IAuthAppService authAppService)
    {
        this.authAppService = authAppService;
    }

    [HttpPost("send-otp")]
    public Task<SendOtpResponseDto> SendOtpAsync([FromBody] SendOtpRequestDto request)
    {
        return authAppService.SendOtpAsync(request);
    }

    [HttpPost("verify-otp")]
    public Task<AuthResponseDto> VerifyOtpAsync([FromBody] VerifyOtpRequestDto request)
    {
        return authAppService.VerifyOtpAsync(request);
    }
}
