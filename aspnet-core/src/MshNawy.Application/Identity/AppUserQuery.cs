using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Application.Contracts.Identity.Admin;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace MshNawy.Application.Identity;

public class AppUserQuery : IAppUserQuery, ITransientDependency
{
    private readonly IAppUserRepository appUserRepository;
    private readonly IFileStorageService fileStorageService;
    private readonly IObjectMapper objectMapper;

    public AppUserQuery(
        IAppUserRepository appUserRepository,
        IFileStorageService fileStorageService,
        IObjectMapper objectMapper)
    {
        this.appUserRepository = appUserRepository;
        this.fileStorageService = fileStorageService;
        this.objectMapper = objectMapper;
    }

    public async Task<KycStatusResponseDto?> GetKycStatusByIdentityUserIdAsync(Guid identityUserId)
    {
        var user = await appUserRepository.FindByIdentityUserIdAsync(identityUserId);
        if (user == null) return null;

        return objectMapper.Map<AppUser, KycStatusResponseDto>(user);
    }

    public async Task<PagedResultDto<AdminKycSubmissionDto>> GetPagedByKycStatusAsync(
        KycStatus status,
        int skipCount,
        int maxResultCount)
    {
        var (items, totalCount) = await appUserRepository.GetPagedByKycStatusAsync(status, skipCount, maxResultCount);

        var dtos = items.Select(x => objectMapper.Map<AppUser, AdminKycSubmissionDto>(x)).ToList();
        return new PagedResultDto<AdminKycSubmissionDto>(totalCount, dtos);
    }

    public async Task<KycImageResponseDto> GetKycImageStreamAsync(Guid identityUserId, string fileToken)
    {
        var user = await appUserRepository.FindByIdentityUserIdAsync(identityUserId);
        if (user == null)
        {
            throw new BusinessException(MshNawyErrorCodes.NotFound);
        }

        var tokens = new[] { user.NationalIdFrontImagePath, user.NationalIdBackImagePath, user.SelfiePath }
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToArray();

        if (!tokens.Contains(fileToken, StringComparer.Ordinal))
        {
            throw new BusinessException(MshNawyErrorCodes.Forbidden);
        }

        var stream = await fileStorageService.GetFileStreamAsync(fileToken);
        var contentType = GetContentType(fileToken);

        return new KycImageResponseDto { Content = stream, ContentType = contentType };
    }

    private static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}
