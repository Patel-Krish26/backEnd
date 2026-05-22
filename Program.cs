using backEnd.Configurations;
using backEnd.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────
// Controllers
// ─────────────────────────────────────────────
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// ─────────────────────────────────────────────
// Database Configuration
// ─────────────────────────────────────────────
builder.Services.AddPostgresDatabase(builder.Configuration);

// ─────────────────────────────────────────────
// JWT Authentication
// ─────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ─────────────────────────────────────────────
// Swagger Configuration
// ─────────────────────────────────────────────
builder.Services.AddSwaggerWithJwt();

// ─────────────────────────────────────────────
// CORS Configuration
// ─────────────────────────────────────────────
builder.Services.AddAngularCors();

// ─────────────────────────────────────────────
// Custom Application Services
// ─────────────────────────────────────────────
builder.Services.AddApplicationServices();

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

    // Swagger opens on root URL
    c.RoutePrefix = string.Empty;
});

// ─────────────────────────────────────────────
// HTTPS Redirection
// ─────────────────────────────────────────────
app.UseHttpsRedirection();

// ─────────────────────────────────────────────
// Static Files
// ─────────────────────────────────────────────
app.UseDefaultFiles();

app.UseStaticFiles();

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
// Render Port Binding
// ─────────────────────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

app.Urls.Add($"http://0.0.0.0:{port}");

// ─────────────────────────────────────────────
// Run Application
// ─────────────────────────────────────────────
app.Run();
