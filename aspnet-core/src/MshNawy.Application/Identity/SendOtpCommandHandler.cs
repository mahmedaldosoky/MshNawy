using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace MshNawy.Application.Identity;

public class SendOtpCommandHandler : IRequestHandler<SendOtpRequestDto, SendOtpResponseDto>, ITransientDependency
{
    private const int MaxAttempts = 5;

    private readonly IAppUserRepository appUserRepository;
    private readonly IOtpService otpService;
    private readonly IOtpSender otpSender;
    private readonly IdentityUserManager identityUserManager;
    private readonly IGuidGenerator guidGenerator;
    private readonly IClock clock;
    private readonly ICurrentTenant currentTenant;

    public SendOtpCommandHandler(
        IAppUserRepository appUserRepository,
        IOtpService otpService,
        IOtpSender otpSender,
        IdentityUserManager identityUserManager,
        IGuidGenerator guidGenerator,
        IClock clock,
        ICurrentTenant currentTenant)
    {
        this.appUserRepository = appUserRepository;
        this.otpService = otpService;
        this.otpSender = otpSender;
        this.identityUserManager = identityUserManager;
        this.guidGenerator = guidGenerator;
        this.clock = clock;
        this.currentTenant = currentTenant;
    }

    public async Task<SendOtpResponseDto> Handle(SendOtpRequestDto request, CancellationToken cancellationToken)
    {
        var identityUser = await identityUserManager.FindByNameAsync(request.PhoneNumber);
        if (identityUser == null)
        {
            identityUser = new IdentityUser(guidGenerator.Create(), request.PhoneNumber, null, currentTenant.Id);
            identityUser.SetPhoneNumber(request.PhoneNumber, true);
            var createResult = await identityUserManager.CreateAsync(identityUser);
            if (!createResult.Succeeded)
            {
                throw new BusinessException(MshNawyErrorCodes.InvalidInput);
            }
        }

        var user = await appUserRepository.FindByIdentityUserIdAsync(identityUser.Id, cancellationToken);
        if (user == null)
        {
            user = new AppUser(guidGenerator.Create(), identityUser.Id, request.PhoneNumber);
            await appUserRepository.InsertAsync(user, autoSave: true, cancellationToken: cancellationToken);
        }

        var otp = otpService.GenerateOtp(user, clock.Now);
        await appUserRepository.UpdateAsync(user, autoSave: true, cancellationToken: cancellationToken);
        await otpSender.SendAsync(request.PhoneNumber, otp);

        return new SendOtpResponseDto
        {
            ExpiresInSeconds = 180,
            AttemptsRemaining = Math.Max(0, MaxAttempts - user.OtpAttemptCount)
        };
    }
}
