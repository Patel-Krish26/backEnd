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
        /// <summary>Registers PostgreSQL DbContext.</summary>
        public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration config)
        {
            var connStr = config.GetConnectionString("DefaultConnection")
                          ?? throw new InvalidOperationException("DefaultConnection string not found in appsettings.json");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connStr));

            return services;
        }

        /// <summary>Registers JWT Bearer authentication.</summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtKey = config["Jwt:Key"] ?? "super_secret_fallback_key_1234567890";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"] ?? "JwtAuthDemo",
                    ValidAudience = config["Jwt:Audience"] ?? "JwtAuthDemoUsers",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            services.AddAuthorization();
            return services;
        }

        /// <summary>Registers Swagger (OpenAPI) with JWT support.</summary>
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FarmEase API",
                    Version = "v1"
                });

                // JWT Authorization configuration for Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your JWT token."
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

            return services;
        }

        /// <summary>Registers all services and repositories via dependency injection.</summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
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

        /// <summary>Configures CORS to allow Angular frontend.</summary>
        public static IServiceCollection AddAngularCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:4200",
                            
                            "https://backend-2a4l.onrender.com/"
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
