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
    Log.Information("Starting MshNawy API Host...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host
        .AddAppSettingsSecretsJson()
        .UseAutofac()
        .UseSerilog();

    await builder.AddApplicationAsync<MshNawy.HttpApi.Host.MshNawyHttpApiHostModule>();

    var app = builder.Build();

    await app.InitializeApplicationAsync();

    Log.Information("MshNawy API Host started successfully.");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "MshNawy API Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;
