using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using MshNawy.Domain;
using MshNawy.Application.Identity;

namespace MshNawy.Application
{
    [DependsOn(typeof(MshNawyDomainModule))]
    public class MshNawyApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<MshNawyApplicationModule>();
            });

            context.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MshNawyApplicationModule).Assembly));
            context.Services.AddValidatorsFromAssembly(typeof(MshNawyApplicationModule).Assembly);
            context.Services.AddTransient<IOtpSender, MockOtpSender>();
            context.Services.AddTransient<IJwtTokenService, JwtTokenService>();
        }
    }
}
