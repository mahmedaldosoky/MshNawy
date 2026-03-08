using Volo.Abp.Modularity;
using MshNawy.Domain.Shared;

namespace MshNawy.Domain
{
    /// <summary>
    /// MshNawy Domain Module - registers domain entities, aggregates, and domain services.
    /// Per Constitution VII: Follows ABP module registration pattern and DDD principles.
    /// </summary>
    [DependsOn(typeof(MshNawyDomainSharedModule))]
    public class MshNawyDomainModule : AbpModule
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
