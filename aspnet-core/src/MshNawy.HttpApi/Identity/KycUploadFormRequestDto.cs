using Microsoft.AspNetCore.Http;

namespace MshNawy.HttpApi.Identity;

public class KycUploadFormRequestDto
{
    public IFormFile? File { get; set; }
    public string FileType { get; set; } = string.Empty;
}
