// SchoolManagement.API/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 
using System.Text;
using SchoolManagement.API.Middleware;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Services.Implementations;
using SchoolManagement.Services.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using SchoolManagement.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration with JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "School Management API", 
        Version = "v1",
        Description = "A comprehensive school management system API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueAppPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Dependency Injection - Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IAdvisorRepository, AdvisorRepository>();
builder.Services.AddScoped<IAcademicianRepository, AcademicianRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExcuseRepository, ExcuseRepository>();
builder.Services.AddScoped<IStudentExamRepository, StudentExamRepository>();
builder.Services.AddScoped<IStudentSubjectRepository, StudentSubjectRepository>();

// Dependency Injection - Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAdvisorService, AdvisorService>();
builder.Services.AddScoped<IAcademicianService, AcademicianService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IExcuseService, ExcuseService>();
builder.Services.AddScoped<IConsistencyService, ConsistencyService>();

// ✅ Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ExamReminderJob>();

// ✅ Hangfire
builder.Services.AddHangfire(c => c.UseMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();

// ⭐ CORS en başta olmalı
app.UseCors("VueAppPolicy");

// Global Exception Handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management API V1");
        c.RoutePrefix = string.Empty;
    });
}

// ✅ Hangfire Dashboard (sadece development'ta açık)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ✅ Hangfire Recurring Job — Her gün 08:00'de çalışır
RecurringJob.AddOrUpdate<ExamReminderJob>(
    "exam-reminder-job",
    job => job.SendRemindersAsync(),
    "0 8 * * *"
);

app.Run();