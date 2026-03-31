using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace MshNawy.Application.Identity;

public class SubmitKycCommandHandler : IRequestHandler<KycSubmitRequestDto, KycStatusResponseDto>, ITransientDependency
{
    private readonly IAppUserRepository appUserRepository;
    private readonly IObjectMapper objectMapper;
    private readonly IClock clock;
    private readonly ICurrentUser currentUser;

    public SubmitKycCommandHandler(
        IAppUserRepository appUserRepository,
        IObjectMapper objectMapper,
        IClock clock,
        ICurrentUser currentUser)
    {
        this.appUserRepository = appUserRepository;
        this.objectMapper = objectMapper;
        this.clock = clock;
        this.currentUser = currentUser;
    }

    public async Task<KycStatusResponseDto> Handle(KycSubmitRequestDto request, CancellationToken cancellationToken)
    {
        if (!currentUser.Id.HasValue)
        {
            throw new BusinessException(MshNawyErrorCodes.Unauthorized);
        }

        var user = await appUserRepository.FindByIdentityUserIdAsync(currentUser.Id.Value, cancellationToken);
        if (user == null)
        {
            throw new BusinessException(MshNawyErrorCodes.NotFound);
        }

        user.SubmitKyc(
            request.FullNameArabic,
            request.DateOfBirth,
            request.NationalIdNumber,
            request.NationalIdFrontToken,
            request.NationalIdBackToken,
            request.SelfieToken,
            clock.Now
        );

        await appUserRepository.UpdateAsync(user, autoSave: true, cancellationToken: cancellationToken);

        return objectMapper.Map<AppUser, KycStatusResponseDto>(user);
    }
}
