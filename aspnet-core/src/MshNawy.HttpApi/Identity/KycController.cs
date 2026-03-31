using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Domain.Shared;

namespace MshNawy.HttpApi.Identity;

[Authorize]
[Route("api/app/kyc")]
public class KycController : AbpController
{
    private readonly IKycAppService kycAppService;

    public KycController(IKycAppService kycAppService)
    {
        this.kycAppService = kycAppService;
    }

    [HttpGet("status")]
    public Task<KycStatusResponseDto> GetStatusAsync()
    {
        return kycAppService.GetStatusAsync();
    }

    [HttpPost("submit")]
    public Task<KycStatusResponseDto> SubmitAsync([FromBody] KycSubmitRequestDto input)
    {
        return kycAppService.SubmitAsync(input);
    }

    [HttpPost("upload")]
    public async Task<KycUploadResponseDto> UploadAsync([FromForm] KycUploadFormRequestDto input)
    {
        await using var stream = input.File?.OpenReadStream() ?? Stream.Null;
        return await kycAppService.UploadAsync(new KycUploadRequestDto
        {
            Content = stream,
            FileName = input.File?.FileName ?? string.Empty,
            ContentType = input.File?.ContentType ?? string.Empty,
            FileType = input.FileType
        });
    }

    [HttpGet("image/{fileToken}")]
    public async Task<IActionResult> GetImageAsync(string fileToken)
    {
        var result = await kycAppService.GetKycImageStreamAsync(fileToken);
        return File(result.Content, result.ContentType);
    }
}
