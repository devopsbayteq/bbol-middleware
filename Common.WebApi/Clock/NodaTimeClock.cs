using Microsoft.Extensions.Configuration;
using NodaTime;

namespace Common.WebApi.Clock;

/// <summary>
/// Constructor
/// </summary>
/// <param name="clock"></param>
public class NodaTimeClock(IConfiguration configuration, NodaTime.IClock clock) : IClock
{
    /// <summary>
    /// Instancia de NodaTime
    /// </summary>
    private readonly NodaTime.IClock _clock = clock;

    /// <summary>
    /// Configuración
    /// </summary>
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Zona Horaria
    /// </summary>
    /// <value></value>
    public DateTimeZone TenantTimeZone { get; set; }

    /// <summary>
    /// </summary>
    public string TimeZone { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <returns></returns>
    public NodaTimeClock(IConfiguration configuration)
        : this(configuration, SystemClock.Instance) => _configuration = configuration;

    public void ConfigureTimeZone(string timeZone)
    {
        TimeZone = string.IsNullOrEmpty(timeZone) ? _configuration.GetSection("TimeZone")?.Value : timeZone;
        TenantTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(TimeZone);
    }

    public DateTime UtcNow()
        => _clock.GetCurrentInstant().InUtc().LocalDateTime.ToDateTimeUnspecified();


    public DateTime Now()
        => _clock.GetCurrentInstant().InZone(TenantTimeZone).LocalDateTime.ToDateTimeUnspecified();

    /// <summary>
    ///  Convierte una fecha en una fecha con el formato de la zona horaria del tenant
    /// </summary>
    /// <param name="date"></param>
    /// <param name="timeZoneSource"></param>
    /// <param name="timeZoneDestination"></param>
    /// <returns></returns>
    public DateTime ConvertDateTimeWithOtherTimeZone(DateTime date, string timeZoneSource, string timeZoneDestination)
    {
        var source = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneSource);
        var localDateTime = LocalDateTime.FromDateTime(date);
        ZonedDateTime fromDateTime = source.AtStrictly(localDateTime);
        var destination = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneDestination);
        return fromDateTime.WithZone(destination).LocalDateTime.ToDateTimeUnspecified();
    }

    /// <summary>
    /// Convierte una fecha en una fecha con el formato de la zona horaria del tenant
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public DateTime UtcToDateTimeWithZone(DateTime dateTime)
    {
        var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
        var dateTimeZone = instant.InZone(TenantTimeZone).ToDateTimeUnspecified();
        return dateTimeZone;
    }
}