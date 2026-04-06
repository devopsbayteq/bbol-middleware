namespace Common.WebApi.Models.AppSettings;

/// <summary>
/// Application settings
/// </summary>
public sealed class AppSetting
{
    /// <summary>
    /// Log sensitive information
    /// </summary>
    public bool LogSensitiveInformation { get; set; } = false;

    /// <summary>
    /// Log headers remove
    /// </summary>
    public List<string> LogHeadersRemove { get; set; } = [];

    /// <summary>
    /// OTP settings
    /// </summary>
    public OtpSettings Otp { get; set; } = new();

    /// <summary>
    /// JWT settings
    /// </summary>
    public JwtSettings Jwt { get; set; } = new();

    /// <summary>
    /// RSA security settings
    /// </summary>
    public RsaSecuritySettings RsaSecurity { get; set; } = new();

    /// <summary>
    /// Certificate settings
    /// </summary>
    public CertificateConfiguration Certificate { get; set; } = new();

    /// <summary>
    /// Integrity validation settings
    /// </summary>
    public IntegrityValidation IntegrityValidation { get; set; } = new();

    /// <summary>
    /// Aes security settings
    /// </summary>
    public AesSecuritySettings AesSecurity { get; set; } = new();

    /// <summary>
    /// Session settings
    /// </summary>
    public SessionSettings Session { get; set; } = new();

    /// <summary>
    /// Validación de login local (contraseña dummy e intentos antes del core).
    /// </summary>
    public LoginSecuritySettings LoginSecurity { get; set; } = new();

    /// <summary>
    /// Time settings
    /// </summary>
    public TimeSettings Time { get; set; } = new();

    /// <summary>
    /// Device activity status validation config
    /// </summary>
    public DeviceActivityStatusValidationConfig DeviceActivityStatusValidationConfig { get; set; } = new();

    /// <summary>
    /// Device root validation config
    /// </summary>
    public DeviceRootValidationConfig DeviceRootValidationConfig { get; set; } = new();

    /// <summary>
    /// Validate duplicate request config
    /// </summary>
    public ValidateDuplicateRequestConfig ValidateDuplicateRequestConfig { get; set; } = new();

    /// <summary>
    /// Versiones de app móvil
    /// </summary>
    public List<VersionConfiguration> VersionsConfiguration { get; set; } = [];
}


/// <summary>
/// Configuraciones para versiones de app móvil
/// </summary>
public class VersionConfiguration
{
    /// <summary>
    /// Plataforma a validar
    /// </summary>
    /// <value></value>
    public PlatformType Platform { get; set; }

    /// <summary>
    /// Validar versión de Aplicación?
    /// </summary>
    /// <value></value>
    public bool Validate { get; set; }

    /// <summary>
    /// Versión mínima soportada
    /// </summary>
    /// <value></value>
    public string Version { get; set; }
}



/// <summary>
/// Configuración de validación de Reqquest Duplicados
/// </summary>
public class ValidateDuplicateRequestConfig
{
    /// <summary>
    /// Habilitado?
    /// </summary>
    /// <value></value>
    public bool Enable { get; set; }

    /// <summary>
    /// Minutos de duración en Cache
    /// </summary>
    /// <value></value>
    public int MinutesDurationCache { get; set; }
}
/// <summary>
/// Configuración de Validación de Root de Dispositivo
/// </summary>
public class DeviceRootValidationConfig
{
    /// <summary>
    /// Plataforma
    /// </summary>
    /// <value></value>
    public Dictionary<string, DeviceRootValidationPlatformConfig> Platform { get; set; }

    /// <summary>
    /// Paths Excluidos
    /// </summary>
    /// <value></value>
    public List<PathConfig> ExcludePaths { get; set; }

    /// <summary>
    /// Configuración de Plataforma
    /// </summary>
    public class DeviceRootValidationPlatformConfig
    {
        /// <summary>
        /// Habilitado?
        /// </summary>
        /// <value></value>
        public bool Enable { get; set; }

        /// <summary>
        /// IsRoot
        /// </summary>
        /// <value></value>
        public bool AllowRoot { get; set; }

        /// <summary>
        /// IsDebugger
        /// </summary>
        /// <value></value>
        public bool AllowDebugger { get; set; }

        /// <summary>
        /// Allow Developer
        /// </summary>
        /// <value></value>
        public bool AllowDevelopmentMode { get; set; }

        /// <summary>
        /// Allow Emulator Device
        /// </summary>
        /// <value></value>
        public bool AllowEmulatorDevice { get; set; }
    }
}

/// <summary>
/// Configuración de Validación de Actividad de Dispositivo
/// </summary>
public class DeviceActivityStatusValidationConfig
{
    /// <summary>
    /// Plataforma
    /// </summary>
    /// <value></value>
    public Dictionary<string, DeviceActivityStatusValidationPlatformConfig> Platform { get; set; }

    /// <summary>
    /// Paths Excluidos
    /// </summary>
    /// <value></value>
    public List<PathConfig> ExcludePaths { get; set; }

    /// <summary>
    /// Configuración de Plataforma
    /// </summary>
    public class DeviceActivityStatusValidationPlatformConfig
    {
        /// <summary>
        /// Habilitado?
        /// </summary>
        /// <value></value>
        public bool Enable { get; set; }

        /// <summary>
        /// Estados Permitidos
        /// </summary>
        /// <value></value>
        public List<string> AllowedStatus { get; set; }
    }
}

/// <summary>
/// Configuración de Path
/// </summary>
public class PathConfig
{
    /// <summary>
    /// Método
    /// </summary>
    /// <value></value>
    public string Method { get; set; }

    /// <summary>
    /// Path
    /// </summary>
    public string Path { get; set; }
}

/// <summary>
/// Time settings
/// </summary>
public sealed class TimeSettings
{
    /// <summary>
    /// Time tolerance in seconds
    /// </summary>
    public int TimeToleranceSeconds { get; set; } = 10;
}

/// <summary>
/// Contraseña dummy y política de bloqueo por intentos fallidos (login usuario/clave).
/// </summary>
public sealed class LoginSecuritySettings
{
    /// <summary>
    /// Contraseña esperada tras descifrado RSA; si no coincide con la enviada, se cuenta intento fallido.
    /// </summary>
    public string DummyPassword { get; set; } = string.Empty;

    /// <summary>
    /// Número máximo de intentos fallidos antes de registrar fecha de bloqueo en el usuario.
    /// </summary>
    public int MaxFailedLoginAttempts { get; set; } = 5;
}

/// <summary>
/// Session settings
/// </summary>
public sealed class SessionSettings
{
    /// <summary>
    /// Session time in seconds
    /// </summary>
    public int SessionTimeSeconds { get; set; }

    /// <summary>
    /// Inactivity timeout in seconds
    /// </summary>
    public int InactivityTimeoutSeconds { get; set; }
}

/// <summary>
/// OTP settings
/// </summary>
public sealed class OtpSettings
{
    /// <summary>
    /// OTP length
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
}

/// <summary>
/// Aes security settings
/// </summary>
public sealed class AesSecuritySettings
{
    /// <summary>
    /// Key
    /// </summary>
    /// <value></value>
    public string Key { get; set; } = string.Empty;
}

/// <summary>
/// Configuración para certificados
/// </summary>
public class CertificateConfiguration
{
    /// <summary>
    /// Hash de certificados Permitidos
    /// </summary>
    /// <value></value>
    public string HashCertificate { get; set; }

    /// <summary>
    /// ValidateHash 
    /// </summary>
    /// <value></value>
    public bool ValidateHash { get; set; }
}