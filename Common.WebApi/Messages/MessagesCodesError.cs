using System.Runtime.Serialization;

namespace Common.WebApi.Messages;

/// <summary>
/// Message codes error for API
/// </summary>
public enum MessageCodes
{
    /// <summary>
    /// Success
    /// </summary>
    [EnumMember(Value = "Operación exitosa")]
    Success = 0,

    /// <summary>
    /// Error del sistema
    /// </summary>
    [EnumMember(Value = "Ocurrio un error interno, por favor intente nuevamente")]
    SystemError = 60,

    /// <summary>
    /// Autorización genérica
    /// </summary>
    [EnumMember(Value = "Ocurrio un error interno, por favor intente nuevamente")]
    AuthorizationGeneric = 62,

    /// <summary>
    /// Error de integridad de datos
    /// </summary>
    [EnumMember(Value = "Ocurrio un error interno, por favor intente nuevamente")]
    ErrorIntegrity = 63,

    /// <summary>
    /// Error de tiempo del cliente
    /// </summary>
    [EnumMember(Value = "Ocurrio un error interno, por favor intente nuevamente")]
    HeaderNotFound = 64,

    /// <summary>
    /// Error de tiempo del cliente
    /// </summary>
    [EnumMember(Value = "Por motivos de seguridad necesitas actualizar la fecha y hora de tu dispositivo")]
    ClientTimeOutdated = 65,

    /// <summary>
    /// Error de tiempo del cliente
    /// </summary>
    [EnumMember(Value = "Ocurrio un error interno, por favor intente nuevamente")]
    HeaderFormatInvalid = 66,

    /// <summary>
    /// Error de estado del dispositivo
    /// </summary>
    [EnumMember(Value = "No minimices ni cambies de aplicación mientras realizas esta operación")]
    DeviceStateNotAllowed = 67,

    /// <summary>
    /// No se encuentra configuración de validación de actividad de dispositivo
    /// </summary>
    [EnumMember(Value = "No se encuentra configuración de validación de actividad de dispositivo")]
    NoDeviceActivityStatusValidationConfig = 68,

    /// <summary>
    /// No se encuentra configuración de validación de root de dispositivo
    /// </summary>
    [EnumMember(Value = "No se encuentra configuración de validación de root de dispositivo")]
    NoDeviceRootValidationConfig = 69,

    /// <summary>
    /// El dispositivo se encuentra en modo Root
    /// </summary>
    [EnumMember(Value = "El dispositivo se encuentra en modo Root")]
    DeviceRootNotAllowed = 70,

    /// <summary>
    /// El dispositivo se encuentra en modo Debugger
    /// </summary>
    [EnumMember(Value = "El dispositivo se encuentra en modo Debugger")]
    DeviceDebuggerNotAllowed = 71,

    /// <summary>
    /// El dispositivo se encuentra en modo Development
    /// </summary>
    [EnumMember(Value = "El dispositivo se encuentra en modo Development")]
    DeviceDevelopmentNotAllowed = 72,

    /// <summary>
    /// El dispositivo está en modo Emulador
    /// </summary>
    [EnumMember(Value = "El dispositivo está en modo Emulador")]
    EmulatorDeviceNotAllowed = 73,

    /// <summary>
    /// Request duplicado
    /// </summary>
    [EnumMember(Value = "Ocurrio un error interno, por favor intente nuevamente")]
    DuplicateRequest = 74,

    /// <summary>
    /// Versión mínima soportada no permitida
    /// </summary>
    [EnumMember(Value = "Existe una versión más actualizada de la aplicación, descárgala para descubrir las nuevas funcionalidades")]
    VersionNotAllowed = 75,

    /// <summary>
    /// No se puede parsear la versión del móvil
    /// </summary>
    [EnumMember(Value = "No se puede parsear la versión del móvil")]
    VersionParseError = 76,

    /// <summary>
    /// No se puede parsear la versión actual de configuración
    /// </summary>
    [EnumMember(Value = "No se puede parsear la versión actual de configuración")]
    VersionConfigurationParseError = 77,

    /// <summary>
    /// Certificados no encontrados
    /// </summary>
    [EnumMember(Value = "Certificados no encontrados")]
    CertificatesNotFound = 100,

    /// <summary>
    /// Datos no coinciden
    /// </summary>
    [EnumMember(Value = "Datos no coinciden")]
    DataDoesNotMatch = 101,

    /// <summary>
    /// Credenciales inválidas
    /// </summary>
    [EnumMember(Value = "Credenciales inválidas")]
    InvalidCredentials = 102,

    /// <summary>
    /// No se pudo resolver el usuario del contexto.
    /// </summary>
    [EnumMember(Value = "Ocurrio un error interno, por favor intente nuevamente")]
    UserContextNotFound = 103,

    /// <summary>
    /// No se permite realizar transacciones mayores a 100.
    /// </summary>
    [EnumMember(Value = "No se permite realizar transacciones mayores a 100.")]
    TransactionAmountGreaterThan100 = 104,

    /// <summary>
    /// No se permite realizar transacciones mayores a 100.
    /// </summary>
    [EnumMember(Value = "El saldo de la cuenta es insuficiente para realizar la transacción.")]
    TransactionAmountInsufficient = 105,

    /// <summary>
    /// El código ingresado es incorrecto.
    /// </summary>
    [EnumMember(Value = "El código ingresado es incorrecto.")]
    OtpIncorrect = 106,

    /// <summary>
    /// La cuenta está bloqueada por intentos fallidos de acceso.
    /// </summary>
    [EnumMember(Value = "Por motivos de seguridad, su cuenta se encuentra bloqueada. Por favor, contacte al banco.")]
    AccountLocked = 107,

    /// <summary>
    /// El alias ya está en uso por otro usuario o coincide con un nombre de usuario existente.
    /// </summary>
    [EnumMember(Value = "El alias indicado no está disponible.")]
    AliasAlreadyInUse = 108,
}

