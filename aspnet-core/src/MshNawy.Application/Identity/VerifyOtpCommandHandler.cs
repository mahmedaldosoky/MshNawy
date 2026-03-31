using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Timing;

namespace MshNawy.Application.Identity;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpRequestDto, AuthResponseDto>, ITransientDependency
{
    private readonly IAppUserRepository appUserRepository;
    private readonly IOtpService otpService;
    private readonly IdentityUserManager identityUserManager;
    private readonly IJwtTokenService jwtTokenService;
    private readonly IClock clock;

    public VerifyOtpCommandHandler(
        IAppUserRepository appUserRepository,
        IOtpService otpService,
        IdentityUserManager identityUserManager,
        IJwtTokenService jwtTokenService,
        IClock clock)
    {
        this.appUserRepository = appUserRepository;
        this.otpService = otpService;
        this.identityUserManager = identityUserManager;
        this.jwtTokenService = jwtTokenService;
        this.clock = clock;
    }

    public async Task<AuthResponseDto> Handle(VerifyOtpRequestDto request, CancellationToken cancellationToken)
    {
        var identityUser = await identityUserManager.FindByNameAsync(request.PhoneNumber);
        if (identityUser == null)
        {
            throw new BusinessException(MshNawyErrorCodes.InvalidCredentials);
        }

        var user = await appUserRepository.FindByIdentityUserIdAsync(identityUser.Id, cancellationToken);
        if (user == null)
        {
            throw new BusinessException(MshNawyErrorCodes.NotFound);
        }

        otpService.VerifyOtp(user, request.OtpCode, clock.Now);
        await appUserRepository.UpdateAsync(user, autoSave: true, cancellationToken: cancellationToken);

        var accessToken = await jwtTokenService.CreateAccessTokenAsync(identityUser);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            UserId = identityUser.Id,
            KycStatus = user.KycStatus
        };
    }
}
