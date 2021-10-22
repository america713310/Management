using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SapsanApp.Helpers;
using SapsanApp.Models;
using SapsanApp.Repositories;
using SapsanApp.Services;
using System;
using System.Collections.Generic;

namespace SapsanApp
{
    public static class ConfigureContainerExtensions
    {
        public static void AddTransientServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(IGenericRepository<,>), typeof(GenericsService<,>));
        }
        public static void AddCorsPolicy(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }
        public static void AddApiVersions(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddApiVersioning(
                config =>
                {
                    config.ReportApiVersions = true;
                    config.AssumeDefaultVersionWhenUnspecified = true;
                    config.DefaultApiVersion = new ApiVersion(1, 0);
                    config.ApiVersionReader = new HeaderApiVersionReader("api-version");
                });
        }
        public static void AddAuthServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // укзывает, будет ли валидироваться издатель при валидации токена
                        ValidateIssuer = true,
                        // строка, представляющая издателя
                        ValidIssuer = AuthOptions.ISSUER,

                        // будет ли валидироваться потребитель токена
                        ValidateAudience = true,
                        // установка потребителя токена
                        ValidAudience = AuthOptions.AUDIENCE,
                        // будет ли валидироваться время существования
                        ValidateLifetime = true,

                        // установка ключа безопасности
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        // валидация ключа безопасности
                        ValidateIssuerSigningKey = true,
                    };
                });
        }
        public static void AddNewtonSoftJsonServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }
        public static void AddSwagger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sapsan", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Scheme = "Bearer",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "Bearer",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                        new List<string>()
                        }
                    });
            });
        }
        public static void AddDbSettings(this IServiceCollection serviceCollection)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();

            serviceCollection.AddDbContextPool<SapsanContext>(options => options.UseMySql(configuration.GetConnectionString("Connection"),
                ServerVersion.AutoDetect(configuration.GetConnectionString("Connection"))));
        }
    }
}