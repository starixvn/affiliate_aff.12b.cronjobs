using Fanex.Athena.AFF12B.CronJobs.Core.Extensions;
using Fanex.Athena.AFF12B.CronJobs.WebApi.WebHost;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
// Add config to the container
builder.Configuration.SetBasePath(env.ContentRootPath);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
var envFromSetting = builder.Configuration.GetAppSettingsValue<string>("Environment");
builder.Configuration.AddJsonFile($"App_Data/Users/Users.{envFromSetting}.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"App_Data/Whitelist/Whitelist.{envFromSetting}.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
app.MapGet("/", () => "Hello, Welcome to New 12B API Service!");

// Configure the HTTP request pipeline.
startup.Configure(app, app.Environment);

app.MapDefaultControllerRoute();

app.Run();
