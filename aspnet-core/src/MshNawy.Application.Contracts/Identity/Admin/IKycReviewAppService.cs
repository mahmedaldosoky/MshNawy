using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using MshNawy.Domain.Shared;

namespace MshNawy.Application.Contracts.Identity.Admin;

public interface IKycReviewAppService : IApplicationService
{
    Task<PagedResultDto<AdminKycSubmissionDto>> GetListAsync(KycStatus status, int skipCount = 0, int maxResultCount = 10);
    Task MoveToUnderReviewAsync(MoveToUnderReviewRequestDto request);
    Task ReviewAsync(ReviewKycRequestDto request);
}
