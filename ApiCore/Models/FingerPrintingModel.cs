namespace ApiCore.Models;
/// <summary>
/// Modelo de fingerprint
/// </summary>
public class FingerPrintingModel
{
    /// <summary>
    /// Estado del dispositivo
    /// </summary>
    public string DeviceState { get; set; }

    /// <summary>
    /// Si es root
    /// </summary>
    public bool IsRoot { get; set; }

    /// <summary>
    /// Si es debugger
    /// </summary>
    public bool IsDebugger { get; set; }

    /// <summary>
    /// Si es desarrollo
    /// </summary>
    public bool IsDevelopment { get; set; }

    /// <summary>
    /// Si es dispositivo físico
    /// </summary>
    public bool IsPhysicalDevice { get; set; }
}