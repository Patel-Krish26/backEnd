using backEnd.Configurations;
using backEnd.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ─── Service Registrations ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Keep ONLY this
builder.Services.AddPostgresDatabase(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();
builder.Services.AddApplicationServices();
builder.Services.AddAngularCors();

// ─── Pipeline Configuration ──────────────────────────────────────────────────
var app = builder.Build();

app.UseGlobalExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FarmEase API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();