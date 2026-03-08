using Volo.Abp.Modularity;
using RealInvest.Domain.Shared;

namespace RealInvest.Domain
{
    /// <summary>
    /// RealInvest Domain Module - registers domain entities, aggregates, and domain services.
    /// Per Constitution VII: Follows ABP module registration pattern and DDD principles.
    /// </summary>
    [DependsOn(typeof(RealInvestDomainSharedModule))]
    public class RealInvestDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);

            // Domain services are registered automatically as public services via attribute-based registration
            // Or manually here if needed:
            // context.Services.AddScoped<LedgerService>();
            // context.Services.AddScoped<BalanceCalculator>();
            // context.Services.AddScoped<FeeCalculator>();
        }
    }
}
