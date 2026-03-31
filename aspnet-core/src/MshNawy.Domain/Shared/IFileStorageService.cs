using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MshNawy.Domain.Shared;

public interface IFileStorageService
{
    Task<string> StoreFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> GetFileStreamAsync(string fileToken, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileToken, CancellationToken cancellationToken = default);
}
