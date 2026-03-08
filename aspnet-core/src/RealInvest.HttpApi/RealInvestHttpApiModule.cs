using Volo.Abp.Modularity;

namespace RealInvest.HttpApi
{
    /// <summary>
    /// RealInvest HttpApi Module - registers API controllers and HTTP-specific configurations.
    /// Per Constitution VII: HTTP/REST layer built on top of application services.
    /// </summary>
    public class RealInvestHttpApiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // API-specific configurations
            // Per Constitution I: All API responses in Arabic only (handled by localization)
        }
    }
}
