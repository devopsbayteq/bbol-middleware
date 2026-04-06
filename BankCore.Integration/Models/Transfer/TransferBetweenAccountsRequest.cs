namespace BankCore.Integration.Models.Transfer;

public class TransferBetweenAccountsRequest
{
    public string Canal { get; set; } = string.Empty;
    public DateTimeOffset Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public TransferAccountInfo Origen { get; set; } = new();
    public TransferDestinationInfo Destino { get; set; } = new();
    public TransferBeneficiaryInfo Beneficiario { get; set; } = new();
    public TransferAmountInfo Montos { get; set; } = new();
    public TransferCurrencyInfo Monedas { get; set; } = new();
    public TransferAuditInfo Auditoria { get; set; } = new();
}

public class TransferAccountInfo
{
    public string TipoCuenta { get; set; } = string.Empty;
    public string Cuenta { get; set; } = string.Empty;
}

public class TransferDestinationInfo : TransferAccountInfo
{
    public string Banco { get; set; } = string.Empty;
}

public class TransferBeneficiaryInfo
{
    public string Nombre { get; set; } = string.Empty;
    public TransferIdentificationInfo Identificacion { get; set; } = new();
}

public class TransferIdentificationInfo
{
    public string Tipo { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
}

public class TransferAmountInfo
{
    public string MontoOrigen { get; set; } = string.Empty;
    public string MontoDestino { get; set; } = string.Empty;
    public string Comision { get; set; } = string.Empty;
}

public class TransferCurrencyInfo
{
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
}

public class TransferAuditInfo
{
    public string Transaccion { get; set; } = string.Empty;
    public string Secuencial { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Oficina { get; set; } = string.Empty;
    public string Depuracion { get; set; } = string.Empty;
    public string UriTransaccion { get; set; } = string.Empty;
}
