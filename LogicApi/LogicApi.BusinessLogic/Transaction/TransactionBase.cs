using System.Globalization;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LogicApi.BusinessLogic.Transaction;

/// <summary>
/// Clase base para operaciones de transacciones
/// </summary>
public abstract class TransactionBase<TRequest, TResponse>(
    ILogger<TransactionBase<TRequest, TResponse>> logger
    ) : BusinessLogicBase(
        logger
        ),
    IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Normaliza el texto a minúsculas y elimina los acentos
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    protected static string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        // 1. Descomponemos (á -> a + ´)
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);

            // Solo permitimos letras (Uppercase, Lowercase, etc.) y números
            // Ignoramos NonSpacingMark (acentos) y otros símbolos
            if (category == UnicodeCategory.UppercaseLetter ||
                category == UnicodeCategory.LowercaseLetter ||
                category == UnicodeCategory.DecimalDigitNumber ||
                c == ' ')
            {
                sb.Append(c);
            }
        }

        // 2. Volvemos a componer y quitamos espacios extra si es necesario
        return sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
    }

}
