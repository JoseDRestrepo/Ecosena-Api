using DotNetEnv;
using EcoSENA.Api.Data;
using EcoSENA.Api.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<EcosenaDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DbConn"), ServerVersion.Parse("8.4")));

builder.Services.AddScoped<IAuthService, AuthService>();

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
