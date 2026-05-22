using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using backEnd.Data;
using backEnd.Helpers;
using backEnd.Interfaces;
using backEnd.Repositories;
using backEnd.Services;

namespace backEnd.Configurations
{
    public static class ServiceExtensions
    {
        // ─────────────────────────────────────────────
        // PostgreSQL Database Configuration
        // ─────────────────────────────────────────────
        public static IServiceCollection AddPostgresDatabase(
            this IServiceCollection services,
            IConfiguration config)
        {
            var connStr =
                Environment.GetEnvironmentVariable("DefaultConnection")
                ?? config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException(
                    "Database connection string not found.");
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connStr, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);

                    npgsqlOptions.CommandTimeout(60);
                });

#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif
            });

            return services;
        }

        // ─────────────────────────────────────────────
        // JWT Authentication
        // ─────────────────────────────────────────────
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration config)
        {
            var jwtKey =
                Environment.GetEnvironmentVariable("JWT_KEY")
                ?? config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key missing");

            var issuer =
                Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? config["Jwt:Issuer"]
                ?? "FarmEase";

            var audience =
                Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                ?? config["Jwt:Audience"]
                ?? "FarmEaseUsers";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;

                options.SaveToken = true;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = issuer,
                        ValidAudience = audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey)),

                        ClockSkew = TimeSpan.Zero
                    };
            });

            services.AddAuthorization();

            return services;
        }

        // ─────────────────────────────────────────────
        // Swagger Configuration
        // ─────────────────────────────────────────────
        public static IServiceCollection AddSwaggerWithJwt(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FarmEase API",
                    Version = "v1",
                    Description = "FarmEase Backend API"
                });

                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "Enter JWT Token like: Bearer {your token}"
                    });

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference =
                                    new OpenApiReference
                                    {
                                        Type =
                                            ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            return services;
        }

        // ─────────────────────────────────────────────
        // Dependency Injection
        // ─────────────────────────────────────────────
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Helpers
            services.AddScoped<JwtHelper>();

            // Repositories
            services.AddScoped<IAgriItemsRepository, AgriItemsRepository>();
            services.AddScoped<IMachineryRepository, MachineryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAgriItemsService, AgriItemsService>();
            services.AddScoped<IMachineryService, MachineryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IMessagesService, MessagesService>();

            return services;
        }

        // ─────────────────────────────────────────────
        // CORS Configuration
        // ─────────────────────────────────────────────
        public static IServiceCollection AddAngularCors(
            this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:4200",
                            "https://farmeaseee.vercel.app"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }
    }
}
