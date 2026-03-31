using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using MshNawy.Domain;
using MshNawy.Domain.Shared;
using MshNawy.Domain.Identity;
using MshNawy.EntityFrameworkCore.Infrastructure.FileStorage;
using MshNawy.EntityFrameworkCore.Repositories;

namespace MshNawy.EntityFrameworkCore
{
    /// <summary>
    /// MshNawy EntityFrameworkCore Module - registers database context and SQL Server provider.
    /// Per Constitution VII: ABP infrastructure layer setup.
    /// Per Constitution II: Double-entry ledger persistence via EF Core with SQL Server.
    /// </summary>
    [DependsOn(
        typeof(MshNawyDomainModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule),
        typeof(AbpIdentityEntityFrameworkCoreModule),
        typeof(AbpPermissionManagementEntityFrameworkCoreModule)
    )]
    public class MshNawyEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MshNawyDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
                options.AddRepository<AppUser, EfCoreAppUserRepository>();
            });

            context.Services.AddTransient<IFileStorageService, LocalFileStorageService>();

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseSqlServer();
            });
        }
    }
}
