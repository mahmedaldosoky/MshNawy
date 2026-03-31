using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.Swashbuckle;
using MshNawy.Application;
using MshNawy.EntityFrameworkCore;
using MshNawy.HttpApi;
using MshNawy.Domain.Shared;
using MshNawy.Application.Identity;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
        typeof(MshNawyHttpApiModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(AbpSwashbuckleModule)
    )]
    public class MshNawyHttpApiHostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);

            var configuration = context.Services.GetConfiguration();

            context.Services.Configure<FileStorageOptions>(
                configuration.GetSection("FileStorage")
            );

            context.Services.Configure<JwtOptions>(
                configuration.GetSection("Jwt")
            );

            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
            context.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            // Add controllers
            context.Services.AddControllers();

            // Configure Swagger
            context.Services.AddAbpSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "MshNawy API",
                    Version = "v1"
                });
                options.DocInclusionPredicate((_, apiDesc) =>
                {
                    var path = apiDesc.RelativePath ?? string.Empty;
                    // Hide unused ABP framework endpoints from Swagger
                    if (path.StartsWith("api/abp/", StringComparison.OrdinalIgnoreCase))
                        return false;
                    return true;
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

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

            // Idempotency middleware for financial endpoints (Constitution §III)
            app.UseMiddleware<IdempotencyMiddleware>();

            app.UseAuditing();

            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "MshNawy API");
                options.RoutePrefix = "swagger";
            });

            app.UseConfiguredEndpoints();
        }
    }
}
