using System.IO;

namespace MshNawy.Application.Contracts.Identity;

public class KycImageResponseDto
{
    public Stream Content { get; set; } = Stream.Null;
    public string ContentType { get; set; } = "application/octet-stream";
}
