using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MshNawy.Application.Contracts.Identity;

public interface IKycAppService : IApplicationService
{
    Task<KycStatusResponseDto> GetStatusAsync();
    Task<KycStatusResponseDto> SubmitAsync(KycSubmitRequestDto input);
    Task<KycUploadResponseDto> UploadAsync(KycUploadRequestDto input);
    Task<KycImageResponseDto> GetKycImageStreamAsync(string fileToken);
}
