using ApiCore.Attributes;
using ApiCore.Config;
using ApiCore.Middleware;
using Autofac;
using BankCore.Integration.Extensions;
using Common.WebApi.ArtificialIntelligence;
using Common.WebApi.Cache.Infrastructure;
using Common.WebApi.Clock;
using Common.WebApi.Models.AppSettings;
using LogicApi.BusinessLogic;
using PersistenceDb.Infrastructure.Extension;
using Serilog;
using Serilog.Core;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ApiCore;

public class Startup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }
    public Logger Logger { get; }

    public Startup(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        if (env.IsDevelopment())
            builder.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);

        builder.AddEnvironmentVariables();

        Configuration = builder.Build();
        Environment = env;

        Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();

        Logger.Information("Environment: {@Environment}", Environment.EnvironmentName);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<AppSetting>().Bind(Configuration).ValidateDataAnnotations();
        services.AddControllers()
          .AddJsonOptions(opt =>
          {
              opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
              opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
          }
               );
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => options.AddSwaggerJwtBearer());
        services.AddArtificialIntelligence();
        services.AddApiVersioning();
        services.AddCorsSetting(Configuration);
        services.AddJwtAuthentication(Configuration);
        services.AddMediatrTypes(typeof(BusinessLogicBase));
        services.AddBankCoreIntegration(Configuration);
        services.AddCustomAttributes();
        services.AddUnitOfWorkRepository();
        services.AddCustomDatabaseConfiguration(Configuration);
        services.AddClock();
        services.AddCustomCache();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCorsSetting(Configuration);
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseRouting();
        app.UseMiddleware<TimeDurationRequestMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<ConfigureContextMiddleware>();
        app.UseMiddleware<ValidateVersionMiddleware>();
        app.UseMiddleware<ValidateClientTimeMiddleware>();
        app.UseMiddleware<ValidateIntegrityMiddleware>();
        app.UseMiddleware<ValidateRootDeviceMiddleware>();
        app.UseMiddleware<ValidateDeviceActivityMiddleware>();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        Logger.Information("Aplicación iniciada correctamente");
    }

    public void ConfigureContainer(ContainerBuilder _)
    {
        // Method intentionally left empty.
    }
}
