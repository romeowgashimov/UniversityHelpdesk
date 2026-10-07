using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Filters;
using Universityhelpdesk.Application.Interfaces;
using Universityhelpdesk.Application.Services;
using Universityhelpdesk.Infrastructure.Data;
using Universityhelpdesk.Infrastructure.Repositories;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    
    // Вывод в консоль
    .WriteTo.Console()

    // Файл бизнес-логики: пишутся только логи приложения UniversityHelpdesk
    .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(Matching.FromSource("Universityhelpdesk"))
    .WriteTo.File("logs/business/app-events.txt", rollingInterval: RollingInterval.Day))
    
    // Системный файл: пишутся только логи фреймворков Microsoft
    .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(Matching.FromSource("Microsoft"))
    .WriteTo.File("logs/system/dotnet-system.txt", rollingInterval: RollingInterval.Day))
    
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var conncetionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(conncetionString));

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("ProductionPolicy"); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}