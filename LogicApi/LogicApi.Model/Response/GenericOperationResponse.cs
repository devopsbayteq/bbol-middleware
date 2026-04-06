namespace LogicApi.Model.Response;
/// <summary>
/// Respuesta Genérica
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="userMessage"></param>
public class GenericCommonOperationResponse(string userMessage)
{
    protected const string SUCCESS = "Operación Exitosa";

    /// <summary>
    /// Mensaje al Usuario
    /// </summary>
    public string UserMessage { get; set; } = userMessage;

    /// <summary>
    /// Operación correcta
    /// </summary>
    /// <returns></returns>
    public static GenericCommonOperationResponse SuccessOperation()
     => new(SUCCESS);
}