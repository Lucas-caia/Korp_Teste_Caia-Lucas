using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NexaFiscal.Inventory.Application.Abstractions;
using NexaFiscal.Inventory.Application.Services;
using NexaFiscal.Inventory.Infrastructure.Persistence;
using NexaFiscal.Inventory.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var frontendOrigin = builder.Configuration["FRONTEND_ORIGIN"] ?? "http://localhost:4200";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "NexaFiscal.Auth";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "NexaFiscal";
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key não configurada.");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));
builder.Services.AddSingleton<IProductRepository, MongoProductRepository>();
builder.Services.AddSingleton<IStockRepository, MongoStockRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<StockService>();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    service = "Nexa Fiscal Inventory Service",
    status = "running"
}));
app.MapHealthChecks("/health");

app.Run();
