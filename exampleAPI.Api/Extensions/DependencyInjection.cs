using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiApi.Application.Handlers;
using MiApi.Application.Mappings;
using MiApi.Application.Validators;
using MiApi.Domain.Interfaces;
using MiApi.Infrastructure.Data;
using MiApi.Infrastructure.Repositories;

namespace MiApi.Api.Extensions
{

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // CQRS / MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(CreateEmployeeCommandHandler)));

        // Mappings
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // Repositorios
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Validaciones
        services.AddValidatorsFromAssemblyContaining(typeof(CreateEmployeeCommandValidator));
        services.AddFluentValidationAutoValidation();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"] 
            ?? throw new InvalidOperationException("La clave 'Jwt:Secret' no está configurada en appsettings.json.");
        
        var key = Encoding.UTF8.GetBytes(jwtSecret);

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
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"] ?? "exampleAPI",
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"] ?? "exampleAPIClient",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "exampleAPI", 
                Version = "v1.01",
                Description = "API RESTful CQRS Redarbor"
            });

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = "Autenticación JWT usando el esquema Bearer. Inserte el token con el formato: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
}
