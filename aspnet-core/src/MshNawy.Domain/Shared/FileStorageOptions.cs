namespace MshNawy.Domain.Shared;

public class FileStorageOptions
{
    public string BasePath { get; set; } = "./uploads";
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    public string[] AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png" };
}
