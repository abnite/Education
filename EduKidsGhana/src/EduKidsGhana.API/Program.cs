using EduKidsGhana.API.Extensions;
using EduKidsGhana.API.Middleware;
using EduKidsGhana.Application.Common;
using EduKidsGhana.Infrastructure.Identity;
using EduKidsGhana.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/edukids-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// JWT Authentication
var jwtKey = builder.Configuration["JwtSettings:Key"] ?? "EduKidsGhana-Super-Secret-Key-2024-Production!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "EduKidsGhana",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "EduKidsGhanaApp",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200", "https://edukidsghana.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// Admin service registration
builder.Services.AddScoped<EduKidsGhana.Application.Interfaces.IAdminService, EduKidsGhana.Infrastructure.Services.Admin.AdminService>();
builder.Services.AddScoped<EduKidsGhana.Application.Interfaces.IParentService, EduKidsGhana.Infrastructure.Services.Curriculum.ParentService>();
builder.Services.AddScoped<EduKidsGhana.Application.Interfaces.ITopicService, EduKidsGhana.Infrastructure.Services.Curriculum.TopicService>();

var app = builder.Build();

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Database seeding failed.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduKids Ghana API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "EduKids Ghana - API Documentation";
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapControllers();

app.MapGet("/", () => "EduKids Ghana API - Learn, Listen, Play, and Grow!");
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", App = "EduKids Ghana", Version = "1.0.0", Timestamp = DateTime.UtcNow }));

Log.Information("EduKids Ghana API started. Learn, Listen, Play, and Grow!");
app.Run();
