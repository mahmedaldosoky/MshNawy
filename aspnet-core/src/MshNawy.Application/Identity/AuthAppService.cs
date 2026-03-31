using System.Threading.Tasks;
using MediatR;
using Volo.Abp.Application.Services;
using MshNawy.Application.Contracts.Identity;

namespace MshNawy.Application.Identity;

public class AuthAppService : ApplicationService, IAuthAppService
{
    private readonly IMediator mediator;

    public AuthAppService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<SendOtpResponseDto> SendOtpAsync(SendOtpRequestDto request)
    {
        return await mediator.Send(request);
    }

    public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request)
    {
        return await mediator.Send(request);
    }
}
