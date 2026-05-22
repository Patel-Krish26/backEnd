using backEnd.Configurations;
using backEnd.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────
// Service Registrations
// ─────────────────────────────────────────────
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Database
builder.Services.AddPostgresDatabase(builder.Configuration);

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Swagger
builder.Services.AddSwaggerWithJwt();

// Application Services
builder.Services.AddApplicationServices();

// CORS
builder.Services.AddAngularCors();

var app = builder.Build();

// ─────────────────────────────────────────────
// Global Exception Middleware
// ─────────────────────────────────────────────
app.UseGlobalExceptionMiddleware();

// ─────────────────────────────────────────────
// Swagger Middleware
// ─────────────────────────────────────────────
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FarmEase API v1");

    // Swagger opens at root URL
    c.RoutePrefix = string.Empty;
});

// ─────────────────────────────────────────────
// IMPORTANT:
// Disable HTTPS redirection on Render
// ─────────────────────────────────────────────
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// ─────────────────────────────────────────────
// CORS
// ─────────────────────────────────────────────
app.UseCors("AllowAngularApp");

// ─────────────────────────────────────────────
// Authentication & Authorization
// ─────────────────────────────────────────────
app.UseAuthentication();

app.UseAuthorization();

// ─────────────────────────────────────────────
// Controllers
// ─────────────────────────────────────────────
app.MapControllers();

// ─────────────────────────────────────────────
// Run Application
// ─────────────────────────────────────────────
app.Run();
