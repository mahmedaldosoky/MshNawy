using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MshNawy.Domain.Shared;
using Volo.Abp.Domain.Repositories;

namespace MshNawy.Domain.Identity;

public interface IAppUserRepository : IRepository<AppUser, Guid>
{
    Task<AppUser?> FindByIdentityUserIdAsync(Guid identityUserId, CancellationToken cancellationToken = default);

    Task<(List<AppUser> Items, int TotalCount)> GetPagedByKycStatusAsync(
        KycStatus status,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default);
}
