using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Targets;
using NLog.Web;
using OfficeOpenXml;
using Quartz;
using Quartz.Impl;
using Server.Controllers;
using Server.Core;
using Server.Core.Email;
using Server.Data;
using Server.Services;
using Server.Services.Jobs;

var builder = WebApplication.CreateBuilder(args);
DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

// Nlog
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var nLogConfigFilePath = Path.Combine(AppContext.BaseDirectory, "nlog.config");
if (!string.IsNullOrEmpty(env) && env != Environments.Production)
{
    nLogConfigFilePath = Path.Combine(AppContext.BaseDirectory, $"nlog.{env}.config");
}

var logger = NLogBuilder.ConfigureNLog(nLogConfigFilePath).GetCurrentClassLogger();

// Create NLog Directory
var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
var logEventInfo = new LogEventInfo
{
    TimeStamp = DateTime.Now
};
var fileName = fileTarget.FileName.Render(logEventInfo);
Directory.CreateDirectory(Path.GetDirectoryName(fileName)!);
builder.Logging.ClearProviders().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.WebHost.UseNLog();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("DBConnectionString"));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleManager>();
builder.Services.AddScoped<TaskManager>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<EmailTemplatingService>();
builder.Services.AddTransient<ITokenService, JwtTokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordService>();

// SINGLETONES.
builder.Services.AddSingleton<EmailConfiguration>();

// Add Controllers to the container.
builder.Services.AddScoped<UserController>();
builder.Services.AddScoped<AuthController>();
builder.Services.AddScoped<TokenController>();
builder.Services.AddScoped<PasswordController>();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

// JOBS
builder.Services.AddScoped<BaseJob, EmailSendingJob>();

// JOB SCHEDULLER.
var schedulerFactory = new StdSchedulerFactory();
var scheduler = schedulerFactory.GetScheduler().Result;
builder.Services.AddSingleton(typeof(IScheduler), scheduler);
scheduler.JobFactory = new JobFactory(builder.Services);
scheduler.Start();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    logger.Debug("Main method execute.");
    logger.Debug($"Environment is {env}");

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex.Message, "Stopped program caz of exception.");
    throw;
}
finally
{
    LogManager.Shutdown();
}