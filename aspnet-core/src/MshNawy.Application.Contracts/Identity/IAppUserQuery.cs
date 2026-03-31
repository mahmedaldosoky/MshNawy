using System;
using System.Threading.Tasks;
using MshNawy.Application.Contracts.Identity.Admin;
using MshNawy.Domain.Shared;
using Volo.Abp.Application.Dtos;

namespace MshNawy.Application.Contracts.Identity;

public interface IAppUserQuery
{
    Task<KycStatusResponseDto?> GetKycStatusByIdentityUserIdAsync(Guid identityUserId);

    Task<PagedResultDto<AdminKycSubmissionDto>> GetPagedByKycStatusAsync(
        KycStatus status,
        int skipCount,
        int maxResultCount);

    Task<KycImageResponseDto> GetKycImageStreamAsync(Guid identityUserId, string fileToken);
}
