using Fanex.Athena.AFF12B.CronJobs.Jobs.WebHost;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
// Add config to the container
builder.Configuration.SetBasePath(env.ContentRootPath);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
app.MapGet("/", () => "Affiliate 12B CronJob is running!");

// Configure the HTTP request pipeline.
startup.Configure(app, app.Environment);

app.MapDefaultControllerRoute();

app.Run();

Fanex.Logging.Logger.Log.Info("Affiliate 12B CronJob started!");
