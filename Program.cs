using backEnd.Configurations;
using backEnd.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddPostgresDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();
builder.Services.AddApplicationServices();
builder.Services.AddAngularCors();

var app = builder.Build();

// Global Exception Middleware
app.UseGlobalExceptionMiddleware();

// Swagger ONLY in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FarmEase API v1");
        c.RoutePrefix = string.Empty;
    });
}

// IMPORTANT FOR RENDER
// DO NOT FORCE HTTPS
// app.UseHttpsRedirection();

app.UseRouting();

// CORS
app.UseCors("AllowAngularApp");

// Authentication
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Health endpoint for Render
app.MapGet("/", () => "FarmEase API Running");

app.Run();
