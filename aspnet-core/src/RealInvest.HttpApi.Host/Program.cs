using Serilog;
using Serilog.Events;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting RealInvest API Host...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host
        .AddAppSettingsSecretsJson()
        .UseAutofac()
        .UseSerilog();

    await builder.AddApplicationAsync<RealInvest.HttpApi.Host.RealInvestHttpApiHostModule>();

    var app = builder.Build();

    await app.InitializeApplicationAsync();

    Log.Information("RealInvest API Host started successfully.");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "RealInvest API Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;
