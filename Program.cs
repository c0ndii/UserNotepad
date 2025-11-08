using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using UserNotepad.Entities;
using UserNotepad.Models;
using UserNotepad.Models.Validators;
using UserNotepad.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddSingleton<ILoggerProvider, NLogLoggerProvider>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("appDb")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IValidator<UserInput>, UserInputValidator>();
builder.Services.AddScoped<IValidator<UserAttributeInput>, UserAttributeInputValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
