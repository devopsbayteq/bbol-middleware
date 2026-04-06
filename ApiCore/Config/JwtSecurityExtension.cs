using System.Text;
using Common.WebApi.Models.AppSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiCore.Config;

public static class JwtSecurityExtension
{
    /// <summary>
    /// Registra validación JWT (Bearer), opciones y emisor de tokens.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException($"Falta la sección de configuración '{JwtSettings.SectionName}'.");

        if (Encoding.UTF8.GetByteCount(jwtSettings.SigningKey) < 32)
            throw new InvalidOperationException("Jwt:SigningKey debe tener al menos 32 bytes (UTF-8) para HS256.");

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();
        return services;
    }

    /// <summary>
    /// Añade el esquema Bearer en Swagger para probar endpoints protegidos.
    /// </summary>
    public static void AddSwaggerJwtBearer(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT: encabezado Authorization con esquema Bearer."
        });
    }
}
