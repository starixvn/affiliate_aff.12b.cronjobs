using System.Diagnostics;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.WebHost;

var webApplicationOptions = new WebApplicationOptions() { 
  Args = args,
  ContentRootPath = AppContext.BaseDirectory
};

var builder = WebApplication.CreateBuilder(webApplicationOptions);

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
var isRunAsService = builder.Configuration.GetAppSettingsValue<bool>("IsRunAsService");

if(isRunAsService) {
  builder.Host.UseWindowsService();
} else {
  builder.Services.AddControllers();

  // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();
}

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
app.MapGet("/", () => "Hello, Affiliate 12B CronJobs are running!");

// Configure the HTTP request pipeline.
startup.Configure(app, app.Environment);

if(!isRunAsService) {
  app.MapDefaultControllerRoute();
}

app.Run();

Fanex.Logging.Logger.Log.Info("Affiliate 12B CronJobs started!");
