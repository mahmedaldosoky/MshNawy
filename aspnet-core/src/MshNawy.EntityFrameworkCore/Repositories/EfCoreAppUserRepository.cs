using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MshNawy.EntityFrameworkCore.Repositories;

public class EfCoreAppUserRepository : EfCoreRepository<MshNawyDbContext, AppUser, Guid>, IAppUserRepository
{
    public EfCoreAppUserRepository(IDbContextProvider<MshNawyDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<AppUser?> FindByIdentityUserIdAsync(Guid identityUserId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId, cancellationToken);
    }

    public async Task<(List<AppUser> Items, int TotalCount)> GetPagedByKycStatusAsync(
        KycStatus status,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(x => x.KycStatus == status);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.KycSubmittedAt)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
