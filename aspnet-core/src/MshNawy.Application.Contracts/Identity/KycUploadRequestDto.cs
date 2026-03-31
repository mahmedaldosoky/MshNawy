using System.IO;
using MediatR;

namespace MshNawy.Application.Contracts.Identity;

public class KycUploadRequestDto : IRequest<KycUploadResponseDto>
{
    public Stream Content { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
}
