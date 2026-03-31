using System.Threading.Tasks;
using MediatR;
using Volo.Abp.Application.Services;
using Volo.Abp;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Domain.Shared;

namespace MshNawy.Application.Identity;

public class KycAppService : ApplicationService, IKycAppService
{
    private readonly IAppUserQuery appUserQuery;
    private readonly IMediator mediator;

    public KycAppService(IAppUserQuery appUserQuery, IMediator mediator)
    {
        this.appUserQuery = appUserQuery;
        this.mediator = mediator;
    }

    public async Task<KycStatusResponseDto> GetStatusAsync()
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new BusinessException(MshNawyErrorCodes.Unauthorized);
        }

        var result = await appUserQuery.GetKycStatusByIdentityUserIdAsync(CurrentUser.Id.Value);
        if (result == null)
        {
            throw new BusinessException(MshNawyErrorCodes.NotFound);
        }

        return result;
    }

    public async Task<KycStatusResponseDto> SubmitAsync(KycSubmitRequestDto input)
    {
        return await mediator.Send(input);
    }

    public async Task<KycUploadResponseDto> UploadAsync(KycUploadRequestDto input)
    {
        return await mediator.Send(input);
    }

    public async Task<KycImageResponseDto> GetKycImageStreamAsync(string fileToken)
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new BusinessException(MshNawyErrorCodes.Unauthorized);
        }

        return await appUserQuery.GetKycImageStreamAsync(CurrentUser.Id.Value, fileToken);
    }
}
