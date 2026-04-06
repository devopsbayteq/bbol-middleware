namespace Common.WebApi.Models.AppSettings;

/// <summary>
/// Configuración de Integridad (sección <c>IntegrityValidation</c> en appsettings o secretos de usuario).
/// </summary>

public sealed class IntegrityValidation
{
    /// <summary>
    /// Ifentificador
    /// </summary>
    public string Identifier { get; set; }

    /// <summary>
    /// Activado o Dessactivado
    /// </summary>
    public bool Enable { get; set; }


    /// <summary>
    /// Throw exception if error
    /// </summary>
    public bool ThrowExceptionIfError { get; set; } = true;
    /// <summary>
    /// Rutas Excluidas
    /// </summary>
    public IEnumerable<string> PathsExclude { get; set; }
}
