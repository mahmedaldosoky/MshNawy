using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using RealInvest.Domain;

namespace RealInvest.EntityFrameworkCore
{
    /// <summary>
    /// RealInvest EntityFrameworkCore Module - registers database context and SQL Server provider.
    /// Per Constitution VII: ABP infrastructure layer setup.
    /// Per Constitution II: Double-entry ledger persistence via EF Core with SQL Server.
    /// </summary>
    [DependsOn(
        typeof(RealInvestDomainModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule)
    )]
    public class RealInvestEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<RealInvestDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseSqlServer();
            });
        }
    }
}
