using Azure.Identity;
using Serilog;
using Serilog.Sinks.ApplicationInsights;
var builder = WebApplication.CreateBuilder(args);

// ✅ Add Key Vault (works for both local + Azure)
try
{
    builder.Configuration.AddAzureKeyVault(
        new Uri("https://kv-dotnet-demo-mysec1.vault.azure.net/"),
        new DefaultAzureCredential());
}
catch (Exception ex)
{
    Console.WriteLine($"Key Vault failed: {ex.Message}");
}

// ✅ Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(
        builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"],
        TelemetryConverter.Traces)
    .CreateLogger();

builder.Host.UseSerilog();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseAuthorization();
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
// ✅ ADD YOUR ROOT ENDPOINT HERE
app.MapGet("/", () => Results.Redirect("/swagger"));

// ✅ Example config endpoint (Phase 6 test)
app.MapGet("/config", (IConfiguration config) =>
{
    var appName = config["MySettings:AppName"];
    var env = config["MySettings:Environment"];

    return $"App: {appName} | Environment: {env}";
});


// ✅ Phase 7 - Key Vault test
app.MapGet("/db", (IConfiguration config) =>
{
    var conn = config.GetConnectionString("Default") ?? "NOT FOUND12";
    return $"Connection: {conn}";
});

// ✅ Logging test endpoint
app.MapGet("/log", () =>
{
    Log.Information("This is a test log from Azure!");
    return "Log sent";
});
app.MapControllers();

app.Run();
