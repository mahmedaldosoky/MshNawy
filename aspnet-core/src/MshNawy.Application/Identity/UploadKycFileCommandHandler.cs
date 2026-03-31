using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MshNawy.Application.Contracts.Identity;
using MshNawy.Domain.Shared;
using Volo.Abp.DependencyInjection;

namespace MshNawy.Application.Identity;

public class UploadKycFileCommandHandler : IRequestHandler<KycUploadRequestDto, KycUploadResponseDto>, ITransientDependency
{
    private readonly IFileStorageService fileStorageService;

    public UploadKycFileCommandHandler(IFileStorageService fileStorageService)
    {
        this.fileStorageService = fileStorageService;
    }

    public async Task<KycUploadResponseDto> Handle(KycUploadRequestDto request, CancellationToken cancellationToken)
    {
        var token = await fileStorageService.StoreFileAsync(request.Content, request.FileName, request.ContentType);
        return new KycUploadResponseDto
        {
            FileToken = token,
            FileType = request.FileType
        };
    }
}
