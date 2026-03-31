using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp;
using MshNawy.Domain.Shared;

namespace MshNawy.EntityFrameworkCore.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageOptions options;

    public LocalFileStorageService(IOptions<FileStorageOptions> options)
    {
        this.options = options.Value;
    }

    public async Task<string> StoreFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (content == null)
        {
            throw new BusinessException(MshNawyErrorCodes.KycImageUploadFailed);
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!options.AllowedExtensions.Contains(extension))
        {
            throw new BusinessException(MshNawyErrorCodes.KycImageInvalid);
        }

        if (content.CanSeek && content.Length > options.MaxFileSizeBytes)
        {
            throw new BusinessException(MshNawyErrorCodes.ConcurrencyFailed);
        }

        Directory.CreateDirectory(options.BasePath);

        var token = $"{Guid.NewGuid():N}{extension}";
        var safeToken = Path.GetFileName(token);
        var path = Path.Combine(options.BasePath, safeToken);

        await using var fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fileStream, cancellationToken);

        return safeToken;
    }

    public Task<Stream> GetFileStreamAsync(string fileToken, CancellationToken cancellationToken = default)
    {
        var safeToken = Path.GetFileName(fileToken);
        if (!string.Equals(fileToken, safeToken, StringComparison.Ordinal))
        {
            throw new BusinessException(MshNawyErrorCodes.Forbidden);
        }

        var path = Path.Combine(options.BasePath, safeToken);
        if (!File.Exists(path))
        {
            throw new BusinessException(MshNawyErrorCodes.NotFound);
        }

        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteFileAsync(string fileToken, CancellationToken cancellationToken = default)
    {
        var safeToken = Path.GetFileName(fileToken);
        if (!string.Equals(fileToken, safeToken, StringComparison.Ordinal))
        {
            throw new BusinessException(MshNawyErrorCodes.Forbidden);
        }

        var path = Path.Combine(options.BasePath, safeToken);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }
}
