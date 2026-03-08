using Volo.Abp.Modularity;
using MshNawy.Domain;

namespace MshNawy.Application
{
    /// <summary>
    /// MshNawy Application Module - registers application services, DTOs, and mapping profiles.
    /// Per Constitution VII: Application layer services coordinate domain logic and persistence.
    /// </summary>
    [DependsOn(typeof(MshNawyDomainModule))]
    public class MshNawyApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // Application services will be registered here as needed for each user story
            // Example: context.Services.AddScoped<IWalletAppService, WalletAppService>();
            // AutoMapper profile registration will be added in Phase 3+
        }
    }
}
