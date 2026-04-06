using ApiCore.Attributes;
using ApiCore.Config;
using ApiCore.Middleware;
using BankCore.Integration.Extensions;
using Common.WebApi.Models.AppSettings;
using LogicApi.BusinessLogic;
using PersistenceDb.Infrastructure.Extension;
using Common.WebApi.Clock;
using Common.WebApi.Cache.Infrastructure;
using Common.WebApi.ArtificialIntelligence;
using System.Text.Json.Serialization;
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

    if (builder.Environment.IsDevelopment())
    {
        builder.Configuration.AddUserSecrets<Program>();
    }

    builder.Configuration.AddEnvironmentVariables();

    var logger = builder.AddSerilogCustom();
    logger.Information("Environment: {@Environment}", builder.Environment.EnvironmentName);
    // Add services to the container.
    builder.Services.AddOptions<AppSetting>().Bind(builder.Configuration).ValidateDataAnnotations();
    builder.Services.AddControllers()
      .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
           );
    builder.Services.AddEndpointsApiExplorer();
    builder.AddSerilogCustom();
    builder.Services.AddSwaggerGen(options => options.AddSwaggerJwtBearer());
    builder.Services.AddArtificialIntelligence();
    builder.Services.AddApiVersioning();
    builder.Services.AddCorsSetting(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddMediatrTypes(typeof(BusinessLogicBase));
    builder.Services.AddBankCoreIntegration(builder.Configuration);
    builder.Services.AddCustomAttributes();
    builder.Services.AddUnitOfWorkRepository();
    builder.Services.AddCustomDatabaseConfiguration(builder.Configuration);
    builder.Services.AddClock();
    builder.Services.AddCustomCache();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseCorsSetting(app.Configuration);
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseMiddleware<TimeDurationRequestMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<ConfigureContextMiddleware>();
    app.UseMiddleware<ValidateVersionMiddleware>();
    app.UseMiddleware<ValidateClientTimeMiddleware>();
    app.UseMiddleware<ValidateIntegrityMiddleware>();
    app.UseMiddleware<ValidateRootDeviceMiddleware>();
    app.UseMiddleware<ValidateDeviceActivityMiddleware>();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

    app.MapControllers();
    logger.Information("Aplicación iniciada correctamente");
    await app.RunAsync();
}
catch (Exception ex)
{
    SerilogExtension.GetLoggerCritical().Error(ex, "Error al Iniciar la Aplicación: {@Message}", ex.Message);
}


