using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using MshNawy.Application;
using MshNawy.EntityFrameworkCore;
using MshNawy.HttpApi;

namespace MshNawy.HttpApi.Host
{
    /// <summary>
    /// MshNawy HttpApi Host Module - main entry point for the API application.
    /// Per Constitution VII: Configures the complete ABP application layering and HTTP host.
    /// Per Constitution VI: Frontend-first delivery - mock API ready for Angular integration.
    /// </summary>
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(MshNawyApplicationModule),
        typeof(MshNawyEntityFrameworkCoreModule),
        typeof(MshNawyHttpApiModule)
    )]
    public class MshNawyHttpApiHostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);

            var configuration = context.Services.GetConfiguration();

            // Add controllers
            context.Services.AddControllers();

            // Configure CORS for Angular frontend
            context.Services.AddCors(options =>
            {
                options.AddPolicy("MshNawyCorsPolicy", builder =>
                {
                    builder
                        .WithOrigins(
                            "http://localhost:4200",
                            "http://localhost:3000",
                            "http://localhost:3001"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            // Development environment setup
            if (context.GetEnvironment().IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();

            // Enable CORS
            app.UseCors("MshNawyCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuditing();

            app.UseConfiguredEndpoints();
        }
    }
}
