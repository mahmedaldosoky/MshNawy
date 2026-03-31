using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Identity;
using MshNawy.Domain.Fees;
using MshNawy.Domain.Identity;
using MshNawy.Domain.Shared;
using MshNawy.Domain.Wallet;

namespace MshNawy.Domain
{
    /// <summary>
    /// MshNawy Domain Module - registers domain entities, aggregates, and domain services.
    /// Per Constitution VII: Follows ABP module registration pattern and DDD principles.
    /// </summary>
    [DependsOn(typeof(MshNawyDomainSharedModule), typeof(AbpIdentityDomainModule))]
    public class MshNawyDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);

            context.Services.AddTransient<IOtpService, OtpService>();
            context.Services.AddTransient<ILedgerService, LedgerService>();
            context.Services.AddTransient<IBalanceCalculator, BalanceCalculator>();
            context.Services.AddTransient<IFeeCalculator, FeeCalculator>();
        }
    }
}
