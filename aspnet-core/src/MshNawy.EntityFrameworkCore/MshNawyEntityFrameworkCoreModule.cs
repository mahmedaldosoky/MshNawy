using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using MshNawy.Domain;

namespace MshNawy.EntityFrameworkCore
{
    /// <summary>
    /// MshNawy EntityFrameworkCore Module - registers database context and SQL Server provider.
    /// Per Constitution VII: ABP infrastructure layer setup.
    /// Per Constitution II: Double-entry ledger persistence via EF Core with SQL Server.
    /// </summary>
    [DependsOn(
        typeof(MshNawyDomainModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule)
    )]
    public class MshNawyEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MshNawyDbContext>(options =>
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
