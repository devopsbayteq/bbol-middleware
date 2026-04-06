namespace Common.WebApi.Clock;
public interface IClock
{
    /// <summary>
    /// Zona horaria por Default
    /// </summary>
    string TimeZone { get; set; }

    /// <summary>
    /// Configura la zona Horaria
    /// </summary>
    /// <param name="timeZone"></param>
    void ConfigureTimeZone(string timeZone);

    /// <summary>
    /// Obtiene la Fecha Utc
    /// </summary>
    /// <returns></returns>
    DateTime UtcNow();

    /// <summary>
    /// Obtiene la fecha Actual configurada la zona horaria
    /// </summary>
    /// <returns></returns>
    DateTime Now();

    /// <summary>
    /// Convierte una fecha en una fecha con el formato de la zona horaria del tenant
    /// </summary>
    /// <param name="date"></param>
    /// <param name="timeZoneSource"></param>
    /// <param name="timeZoneDestination"></param>
    /// <returns></returns>
    DateTime ConvertDateTimeWithOtherTimeZone(DateTime date, string timeZoneSource, string timeZoneDestination);

    /// <summary>
    /// Convierte una fecha utc a otra fecha con el formato de la zona horaria del tenant
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    DateTime UtcToDateTimeWithZone(DateTime dateTime);
}