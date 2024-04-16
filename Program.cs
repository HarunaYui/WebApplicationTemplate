using System.Reflection;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using NLog.Web;
using WebApplicationTemplate.ActionFilter;
using WebApplicationTemplate.JWT;
using WebApplicationTemplate.Model.Entity;
using WebApplicationTemplate.Model.From;
using WebApplicationTemplate.MyHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
//var dbconfig = config["ConnectionStrings:MariaDbConnectionString"];
//if (string.IsNullOrEmpty(dbconfig))
//{
//    Console.WriteLine("db null");
//    return;
//}

builder.Services.AddTransient<MySqlConnection>(x =>
    new(builder.Configuration.GetConnectionString("MariaDbConnectionString")));
//builder.Services.AddTransient<AppDB>(sp => new(dbconfig));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header.\r\nExample:'Bearer xxxxx'",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Authorization" },
        Scheme = "oauth2",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Authorization", scheme);
    var requirement = new OpenApiSecurityRequirement();
    requirement[scheme] = new List<string>();
    c.AddSecurityRequirement(requirement);
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Template",
        Version = "v1",
        Description = "WebApi Template",
    });
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "WebTemplate.xml");
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
    c.OrderActionsBy(o => o.RelativePath);
});
builder.Services.AddMemoryCache();
builder.Services.Configure<DBDataBase>(builder.Configuration.GetSection("DBSetting:DBTables"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    var jwtopt = builder.Configuration.GetSection("JWT").Get<JwtSettings>();
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
builder.Services.AddScoped<IValidator<UserRegister>, UserRegisterValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || config.GetValue("Swagger:Enable", false))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "WepApiģ��";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "�û�ģ��");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<MyHub>("/MyHub");

app.MapControllers();

app.Run();
