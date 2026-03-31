using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MshNawy.Application.Contracts.Identity.Admin;
using MshNawy.Domain.Identity;
using Volo.Abp.DependencyInjection;

namespace MshNawy.Application.Identity.Admin;

public class MoveToUnderReviewCommandHandler : IRequestHandler<MoveToUnderReviewRequestDto>, ITransientDependency
{
    private readonly IAppUserRepository appUserRepository;

    public MoveToUnderReviewCommandHandler(IAppUserRepository appUserRepository)
    {
        this.appUserRepository = appUserRepository;
    }

    public async Task Handle(MoveToUnderReviewRequestDto request, CancellationToken cancellationToken)
    {
        var user = await appUserRepository.GetAsync(request.UserId, cancellationToken: cancellationToken);
        user.MoveToUnderReview();
        await appUserRepository.UpdateAsync(user, autoSave: true, cancellationToken: cancellationToken);
    }
}
