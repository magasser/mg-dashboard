using System.Text;

using MG.Dashboard.Api.Context;
using MG.Dashboard.Api.Services;
using MG.Dashboard.Env;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Serilog;

DotEnv.Load(".env");
DotEnv.Load(".env.development");

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();

builder.Configuration.AddEnvironmentVariables();

builder.Logging
       .ClearProviders()
       .AddSerilog(logger);

builder.Services.AddControllers();

builder.Services
       .AddApiVersioning(
           options =>
           {
               options.ApiVersionReader = new HeaderApiVersionReader("api-version");
               options.ReportApiVersions = true;
               options.AssumeDefaultVersionWhenUnspecified = true;
               options.DefaultApiVersion = new ApiVersion(1, 0);
           })
       .AddEndpointsApiExplorer()
       .AddSwaggerGen(options => options.CustomSchemaIds(type => type.FullName))
       .AddDbContext<MgDashboardContext>(
           options => options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")))
       .AddScoped<IUserService, UserService>()
       .AddScoped<IDeviceService, DeviceService>()
       .AddSingleton<ITokenService, TokenService>();

builder.Services
       .AddAuthorization()
       .AddAuthentication(
           options =>
           {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
           })
       .AddJwtBearer(
           options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                   ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                   IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!)),
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = false,
                   ValidateIssuerSigningKey = true
               };
           });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}

app.UseCors(builder => builder.AllowAnyOrigin()
                              .SetIsOriginAllowed(_ => true)
                              .AllowAnyHeader()
                              .AllowAnyMethod());

app.UseAuthentication()
   .UseAuthorization();

app.MapControllers();

app.Run();