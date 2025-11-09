using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Text;
using UserNotepad.Entities;
using UserNotepad.Filters;
using UserNotepad.Middlewares;
using UserNotepad.Models;
using UserNotepad.Models.Validators;
using UserNotepad.Services;
using UserNotepad.Settings;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowUI", policy =>
        {
            policy.WithOrigins(@"http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<Jwt>();
    if (jwtSettings is null)
        throw new InvalidOperationException("Missing jwt settings section in appsettings.json");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options  =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var dbConnectionString = builder.Configuration.GetConnectionString("appDb");
    if (dbConnectionString is null)
        throw new InvalidOperationException("Missing database section in appsettings.json");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(dbConnectionString));

    builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IValidator<UserInput>, UserInputValidator>();
    builder.Services.AddScoped<IValidator<UserAttributeInput>, UserAttributeInputValidator>();
    builder.Services.AddScoped<IValidator<RegisterInput>, RegisterInputValidator>();
    builder.Services.AddScoped<IValidator<LoginInput>, LoginInputValidator>();
    builder.Services.AddScoped<IPasswordHasher<Operator>, PasswordHasher<Operator>>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors("AllowUI");

    app.UseAuthentication();
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