using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MshNawy.EntityFrameworkCore
{
    /// <summary>
    /// Design-time factory for EF Core tooling to avoid booting the full host.
    /// </summary>
    public class MshNawyDbContextFactory : IDesignTimeDbContextFactory<MshNawyDbContext>
    {
        public MshNawyDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.GetFullPath(
                Path.Combine(Directory.GetCurrentDirectory(), "..", "MshNawy.HttpApi.Host"));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'Default' is missing.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<MshNawyDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MshNawyDbContext(optionsBuilder.Options);
        }
    }
}
