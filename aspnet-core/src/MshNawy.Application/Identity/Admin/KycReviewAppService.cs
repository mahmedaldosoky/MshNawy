using System;
using System.Threading.Tasks;
using MediatR;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Application.Contracts.Identity.Admin;
using MshNawy.Domain.Shared;

namespace MshNawy.Application.Identity.Admin;

public class KycReviewAppService : ApplicationService, IKycReviewAppService
{
    private readonly IAppUserQuery appUserQuery;
    private readonly IMediator mediator;

    public KycReviewAppService(IAppUserQuery appUserQuery, IMediator mediator)
    {
        this.appUserQuery = appUserQuery;
        this.mediator = mediator;
    }

    public async Task<PagedResultDto<AdminKycSubmissionDto>> GetListAsync(KycStatus status, int skipCount = 0, int maxResultCount = 10)
    {
        return await appUserQuery.GetPagedByKycStatusAsync(status, skipCount, maxResultCount);
    }

    public async Task MoveToUnderReviewAsync(MoveToUnderReviewRequestDto request)
    {
        await mediator.Send(request);
    }

    public async Task ReviewAsync(ReviewKycRequestDto request)
    {
        await mediator.Send(request);
    }
}
