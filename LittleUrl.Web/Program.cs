using Microsoft.EntityFrameworkCore;
using LittleUrl.Domain.Repositories;
using LittleUrl.Infrastructure.EntityFramework;
using LittleUrl.Infrastructure;
using LittleUrl.Website.Services;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// clear the existing loggers
builder.Logging.ClearProviders();

// allow to pull data from the appsettings file
var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

// define the Serilog Logger
var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

// get the system to use Serilog as the logger
Log.Logger = logger;
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// DI for Db Context
builder.Services.AddDbContext<LittleUrlDbContext>(options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI for Services
builder.Services.AddScoped<ILitlUrlService, LitlUrlService>();

// DI for Repository
builder.Services.AddScoped<ILitlUrlRepository, LitlUrlRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
