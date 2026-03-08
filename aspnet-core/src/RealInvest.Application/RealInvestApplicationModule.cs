using Volo.Abp.Modularity;
using RealInvest.Domain;

namespace RealInvest.Application
{
    /// <summary>
    /// RealInvest Application Module - registers application services, DTOs, and mapping profiles.
    /// Per Constitution VII: Application layer services coordinate domain logic and persistence.
    /// </summary>
    [DependsOn(typeof(RealInvestDomainModule))]
    public class RealInvestApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // Application services will be registered here as needed for each user story
            // Example: context.Services.AddScoped<IWalletAppService, WalletAppService>();
            // AutoMapper profile registration will be added in Phase 3+
        }
    }
}
