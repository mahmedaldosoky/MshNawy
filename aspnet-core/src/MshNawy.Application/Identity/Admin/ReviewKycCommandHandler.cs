using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MshNawy.Application.Contracts.Identity.Admin;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace MshNawy.Application.Identity.Admin;

public class ReviewKycCommandHandler : IRequestHandler<ReviewKycRequestDto>, ITransientDependency
{
    private readonly IAppUserRepository appUserRepository;

    public ReviewKycCommandHandler(IAppUserRepository appUserRepository)
    {
        this.appUserRepository = appUserRepository;
    }

    public async Task Handle(ReviewKycRequestDto request, CancellationToken cancellationToken)
    {
        var user = await appUserRepository.GetAsync(request.UserId, cancellationToken: cancellationToken);

        switch (request.Input.Decision)
        {
            case KycReviewDecision.Approve:
                user.ApproveKyc();
                break;
            case KycReviewDecision.Reject:
                user.RejectKyc(request.Input.Reason!);
                break;
            case KycReviewDecision.NeedsResubmission:
                user.RequestResubmission(request.Input.Reason!);
                break;
            default:
                throw new BusinessException(MshNawyErrorCodes.InvalidInput);
        }

        await appUserRepository.UpdateAsync(user, autoSave: true, cancellationToken: cancellationToken);
    }
}
