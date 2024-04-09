using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using WebApplicationTemplate.ActionFilter;
using WebApplicationTemplate.AppDB;
using WebApplicationTemplate.Entity;
using WebApplicationTemplate.FluentValidation;
using WebApplicationTemplate.JWT;
using WebApplicationTemplate.MyHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var dbconfig = config["ConnectionStrings:MariaDbConnectionString"];
if (string.IsNullOrEmpty(dbconfig))
{
    Console.WriteLine("db null");
    return;
}

builder.Services.AddTransient<AppDB>(sp => new(dbconfig));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header.\r\nExample:'Bearer xxxxx'",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Authorization" },
        Scheme = "oauth2", Name = "Authorization",
        In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Authorization",scheme);
    var requirement = new OpenApiSecurityRequirement();
    requirement[scheme] = new List<string>();
    c.AddSecurityRequirement(requirement);
});
builder.Services.AddMemoryCache();
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    var jwtopt = builder.Configuration.GetSection("JWT").Get<JWTSettings>();
    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtopt.SecrectKey);
    var secKey = new SymmetricSecurityKey(keyBytes);
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = secKey
    };
});
builder.Services.Configure<MvcOptions>(opt =>
{
    opt.Filters.Add<RateLimitFilter>();
    opt.Filters.Add<TransactionScopeFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddSignalR();
builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IValidator<User>, UserRequestValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || config.GetValue("Swagger:Enable", false))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<MyHub>("/MyHub");

app.MapControllers();

app.Run();
