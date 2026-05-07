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

    // JWT Authentication in Swagger
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

// ⭐ CORS Configuration - Vue Frontend İçin
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueAppPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost") // Bütün localhost portlarına izin verir
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

var app = builder.Build();

// ⭐ ÖNEMLI: CORS middleware sırası kritik!
app.UseCors("VueAppPolicy");  // CORS en başta olmalı

// Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseStaticFiles();


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management API V1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}


//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();