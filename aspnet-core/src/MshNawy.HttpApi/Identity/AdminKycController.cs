using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using MshNawy.Application.Contracts.Identity.Admin;
using MshNawy.Domain.Shared;

namespace MshNawy.HttpApi.Identity;

[Authorize]
[Route("api/app/admin/kyc")]
public class AdminKycController : AbpController
{
    private readonly IKycReviewAppService kycReviewAppService;

    public AdminKycController(IKycReviewAppService kycReviewAppService)
    {
        this.kycReviewAppService = kycReviewAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<AdminKycSubmissionDto>> GetListAsync(
        [FromQuery] KycStatus status,
        [FromQuery] int skipCount = 0,
        [FromQuery] int maxResultCount = 10)
    {
        return kycReviewAppService.GetListAsync(status, skipCount, maxResultCount);
    }

    [HttpPost("{userId:guid}/review")]
    public Task ReviewAsync([FromRoute] Guid userId, [FromBody] ReviewKycRequestDto request)
    {
        request.UserId = userId;
        return kycReviewAppService.ReviewAsync(request);
    }
}
