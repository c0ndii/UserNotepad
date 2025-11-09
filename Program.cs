using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using UserNotepad.Entities;
using UserNotepad.Filters;
using UserNotepad.Middlewares;
using UserNotepad.Models;
using UserNotepad.Models.Validators;
using UserNotepad.Services;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);


    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("appDb")));

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IValidator<UserInput>, UserInputValidator>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddScoped<IValidator<UserAttributeInput>, UserAttributeInputValidator>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseMiddleware<ErrorHandlingMiddleware>();

    app.MapControllers();

    app.Run();

}
catch (Exception e)
{
    logger.Error(e, "App could not start");
    throw;
}
finally
{
    LogManager.Shutdown();
}