using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace MshNawy.HttpApi
{
    /// <summary>
    /// MshNawy HttpApi Module - registers API controllers and HTTP-specific configurations.
    /// Per Constitution VII: HTTP/REST layer built on top of application services.
    /// </summary>
    [DependsOn(typeof(AbpAspNetCoreMvcModule))]
    public class MshNawyHttpApiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // API-specific configurations
            // Per Constitution I: All API responses in Arabic only (handled by localization)
        }
    }
}
