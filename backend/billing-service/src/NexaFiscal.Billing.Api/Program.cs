var builder = WebApplication.CreateBuilder(args);

var frontendOrigin = builder.Configuration["FRONTEND_ORIGIN"] ?? "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors();

app.MapGet("/", () => Results.Ok(new
{
    service = "Nexa Fiscal Billing Service",
    status = "running"
}));

app.MapHealthChecks("/health");

app.Run();
