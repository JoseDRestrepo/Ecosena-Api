using CloudinaryDotNet;
using DotNetEnv;
using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<EcosenaDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DbConn"), ServerVersion.Parse("8.4")));

builder.Services.AddDbContext<SofiaDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SenaConn"), ServerVersion.Parse("8.4")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!))
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("auth", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("general", config =>
    {
        config.PermitLimit = 60;                         
        config.Window = TimeSpan.FromMinutes(1);         
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 5;
    });

    options.AddFixedWindowLimiter("uploads", config =>
    {
        config.PermitLimit = 10;                         
        config.Window = TimeSpan.FromMinutes(1);         
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });
});

var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");

var cloudinary = new Cloudinary(new Account(
        cloudinaryConfig["CloudName"],
        cloudinaryConfig["ApiKey"],
        cloudinaryConfig["ApiSecret"]
    ));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IBlogService,  BlogService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddSingleton(cloudinary);
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddSingleton<ICensorshipService, CensorshipService>();
builder.Services.AddScoped<IPenalizacionService,  PenalizacionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
