using System.Globalization;
using BankCore.Integration.Interfaces;
using BankCore.Integration.Models.Transfer;
using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using LogicApi.Model.Request.Transaction;
using LogicApi.Model.Response.Transaction;
using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Core;
using PersistenceDb.Models.Enums;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using CoreTransaction = PersistenceDb.Models.Core.Transaction;
using CoreBeneficiary = PersistenceDb.Models.Core.Beneficiary;
using Common.WebApi.Clock;

namespace LogicApi.BusinessLogic.Transaction;

/// <summary>
/// Handler para crear transferencia
/// </summary>
public class CreateTransferHandler(
    ILogger<CreateTransferHandler> logger,
    IUnitOfWork unitOfWork,
    IClock clock,
    IBankCoreServices bankCoreServices
    ) : TransactionBase<CreateTransferRequest, CreateTransferResponse>(logger)
{
    public override async Task<CreateTransferResponse> Handle(CreateTransferRequest request, CancellationToken cancellationToken)
    {
        if (request.Amount > 100m)
            throw new CustomException(MessageCodes.TransactionAmountGreaterThan100, "No se permite realizar transacciones mayores a 100.");

        if (!Guid.TryParse(request.AccountGuid, out var accountGuid))
            throw new CustomException(MessageCodes.SystemError, "El guid de cuenta es inválido.");
        if (!Guid.TryParse(request.BeneficiaryContactGuid, out var beneficiaryGuid))
            throw new CustomException(MessageCodes.SystemError, "El guid de beneficiario es inválido.");

        var accountExists = await unitOfWork.AccountUserRepository
            .ExistAnyAsync(a => a.Guid == accountGuid)
            .ConfigureAwait(false);
        if (!accountExists)
            throw new CustomException(MessageCodes.SystemError, "La cuenta origen no existe.");

        var beneficiaryExists = await unitOfWork.BeneficiaryRepository
            .ExistAnyAsync(b => b.Id == beneficiaryGuid)
            .ConfigureAwait(false);
        if (!beneficiaryExists)
            throw new CustomException(MessageCodes.SystemError, "El beneficiario no existe.");

        var accountTransactions = await unitOfWork.TransactionRepository
            .SumAsync(
                sum => sum.Amount,
                where => where.AccountGuid == accountGuid)
            .ConfigureAwait(false);
        if (request.Amount > accountTransactions)
            throw new CustomException(MessageCodes.TransactionAmountInsufficient, $"El saldo: {accountTransactions} de la cuenta es insuficiente para realizar la transacción.");

        var account = await unitOfWork.AccountUserRepository
            .GetByFirstOrDefaultAsync(a => a.Guid == accountGuid)
            .ConfigureAwait(false)
            ?? throw new CustomException(MessageCodes.SystemError, "La cuenta origen no existe.");
        var beneficiary = await unitOfWork.BeneficiaryRepository
            .GetByFirstOrDefaultAsync(b => b.Id == beneficiaryGuid)
            .ConfigureAwait(false) ?? throw new CustomException(MessageCodes.SystemError, "El beneficiario no existe.");
        var bankCoreRequest = BuildBankCoreTransferRequest(request, account, beneficiary);

        TransferBetweenAccountsResponse coreResponse;
        try
        {
            coreResponse = await bankCoreServices
                .TransferBetweenAccountsAsync(bankCoreRequest, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Error al procesar la transferencia en BankCore.");
            throw new CustomException(MessageCodes.SystemError, "No se pudo completar la transferencia con el core bancario.");
        }

        if (coreResponse is null || string.IsNullOrWhiteSpace(coreResponse.SecuenciaTransaccion))
            throw new CustomException(MessageCodes.SystemError, "El core bancario no devolvió un identificador de transacción válido.");

        var externalId = coreResponse.SecuenciaTransaccion.Trim();

        await unitOfWork.TransactionRepository.AddAsync(new CoreTransaction
        {
            Id = Guid.NewGuid(),
            RegisterDate = clock.Now(),
            AccountGuid = accountGuid,
            BeneficiaryGuid = beneficiaryGuid,
            Amount = -Math.Abs(request.Amount),
            Description = request.Concept ?? string.Empty,
            ExternalIdentifier = externalId,
            AbsoluteAmount = Math.Abs(request.Amount),
            BeforeTransactionAmount = accountTransactions,
            ResultingAmount = accountTransactions - request.Amount,
            TransactionType = (byte)TransactionType.SentTransfers,
            NormalizedDescription = NormalizeText(request.Concept)
        }).ConfigureAwait(false);

        return new CreateTransferResponse
        {
            TransactionIdentifier = externalId
        };
    }

    private static TransferBetweenAccountsRequest BuildBankCoreTransferRequest(
        CreateTransferRequest request,
        AccountUser account,
        CoreBeneficiary beneficiary)
    {
        var amountStr = request.Amount.ToString(CultureInfo.InvariantCulture);
        var clientDate = request.ContextRequest?.Headers?.ClientDate;
        var fecha = clientDate.HasValue
            ? new DateTimeOffset(DateTime.SpecifyKind(clientDate.Value, DateTimeKind.Utc))
            : DateTimeOffset.UtcNow;

        return new TransferBetweenAccountsRequest
        {
            Canal = "MB",
            Fecha = fecha,
            Tipo = "TRF",
            Concepto = request.Concept,
            Origen = new TransferAccountInfo
            {
                TipoCuenta = MapAccountTypeCode(account.AccountType),
                Cuenta = account.AccountNumber
            },
            Destino = new TransferDestinationInfo
            {
                TipoCuenta = MapBeneficiaryAccountTypeCode(beneficiary.AccountType),
                Cuenta = beneficiary.AccountNumber,
                Banco = beneficiary.BankName
            },
            Beneficiario = new TransferBeneficiaryInfo
            {
                Nombre = beneficiary.Name,
                Identificacion = new TransferIdentificationInfo
                {
                    Tipo = "C",
                    Numero = beneficiary.Identification
                }
            },
            Montos = new TransferAmountInfo
            {
                MontoOrigen = amountStr,
                MontoDestino = amountStr,
                Comision = "0"
            },
            Monedas = new TransferCurrencyInfo
            {
                Origen = "USD",
                Destino = "USD"
            },
            Auditoria = new TransferAuditInfo
            {
                Transaccion = request.ContextRequest?.RequestId ?? string.Empty,
                Secuencial = request.ContextRequest?.RequestId ?? Guid.NewGuid().ToString("N"),
                Usuario = request.ContextRequest?.CustomClaims?.UserName ?? string.Empty,
                Oficina = string.Empty,
                Depuracion = string.Empty,
                UriTransaccion = string.Empty
            }
        };
    }

    private static string MapAccountTypeCode(AccountType type) => type switch
    {
        AccountType.Savings => "1",
        AccountType.Checking => "2",
        _ => ((byte)type).ToString(CultureInfo.InvariantCulture)
    };

    private static string MapBeneficiaryAccountTypeCode(byte accountType) =>
        accountType.ToString(CultureInfo.InvariantCulture);
}
