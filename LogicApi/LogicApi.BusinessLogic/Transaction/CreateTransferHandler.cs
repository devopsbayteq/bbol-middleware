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
using BankCore.Integration.Exceptions;

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
        var originAccount = await unitOfWork.AccountUserRepository
            .GetByFirstOrDefaultAsync(a => a.Guid == request.AccountGuid)
            .ConfigureAwait(false) ?? throw new CustomException(MessageCodes.SystemError, "La cuenta origen no existe.");
        var beneficiary = await unitOfWork.BeneficiaryRepository
            .GetByFirstOrDefaultAsync(b => b.Id == request.BeneficiaryContactGuid && b.UserId == originAccount.UserId)
            .ConfigureAwait(false);

        var accountTransactions = await unitOfWork.TransactionRepository
            .SumAsync(
                sum => sum.Amount,
                where => where.AccountGuid == request.AccountGuid)
            .ConfigureAwait(false);
        if (request.Amount > accountTransactions)
            throw new CustomException(MessageCodes.TransactionAmountInsufficient, $"El saldo: {accountTransactions} de la cuenta es insuficiente para realizar la transacción.");

        var bankCoreRequest = BuildBankCoreTransferRequest(request, originAccount, beneficiary);

        TransferBetweenAccountsResponse coreResponse;
        try
        {
            coreResponse = await bankCoreServices
                .TransferBetweenAccountsAsync(bankCoreRequest, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (BankCoreUserMessageException ex)
        {
            logger.LogError(ex, "Error al procesar la transferencia en BankCore.");
            throw new CustomException(MessageCodes.BankCoreUserMessage, ex.Message);
        }

        if (coreResponse is null || string.IsNullOrWhiteSpace(coreResponse.SecuenciaTransaccion))
            throw new CustomException(MessageCodes.SystemError, "El core bancario no devolvió un identificador de transacción válido.");

        var externalId = coreResponse.SecuenciaTransaccion.Trim();
        await unitOfWork.BeginTransactionAsync().ConfigureAwait(false);
        await unitOfWork.TransactionRepository.AddAsync(new CoreTransaction
        {
            Id = Guid.NewGuid(),
            RegisterDate = clock.Now(),
            AccountGuid = request.AccountGuid,
            BeneficiaryGuid = request.BeneficiaryContactGuid,
            Amount = -Math.Abs(request.Amount),
            Description = request.Concept ?? string.Empty,
            ExternalIdentifier = externalId,
            AbsoluteAmount = Math.Abs(request.Amount),
            BeforeTransactionAmount = accountTransactions,
            ResultingAmount = accountTransactions - request.Amount,
            TransactionType = (byte)TransactionType.SentTransfers,
            NormalizedDescription = NormalizeText(request.Concept)
        }).ConfigureAwait(false);
        //Si no es beneficiario, se debe crear una transacción entre cuentas propias
        var ownerBeneficiary = await unitOfWork.AccountUserRepository
            .GetByFirstOrDefaultAsync(a => a.AccountNumber == beneficiary.AccountNumber && a.UserId == originAccount.UserId)
            .ConfigureAwait(false) ??
            throw new CustomException(MessageCodes.SystemError, "El beneficiario no es propietario de la cuenta.");
        if (ownerBeneficiary is not null)
        {
            var ownerAccountTransactions = await unitOfWork.TransactionRepository
                .SumAsync(
                    sum => sum.Amount,
                    where => where.AccountGuid == ownerBeneficiary.Guid)
                .ConfigureAwait(false);
            var originBenficiaryAccount = await unitOfWork.BeneficiaryRepository.GetFirstOrDefaultGenericAsync(
                select => select.Id,
                where => where.AccountNumber == originAccount.AccountNumber && where.UserId == originAccount.UserId).ConfigureAwait(false);

            await unitOfWork.TransactionRepository.AddAsync(new CoreTransaction
            {
                Id = Guid.NewGuid(),
                RegisterDate = clock.Now(),
                AccountGuid = ownerBeneficiary.Guid,
                BeneficiaryGuid = originBenficiaryAccount,
                Amount = Math.Abs(request.Amount),
                Description = request.Concept ?? string.Empty,
                ExternalIdentifier = externalId,
                AbsoluteAmount = Math.Abs(request.Amount),
                BeforeTransactionAmount = ownerAccountTransactions,
                ResultingAmount = ownerAccountTransactions + request.Amount,
                TransactionType = (byte)TransactionType.ReceivedTransfers,
                NormalizedDescription = NormalizeText(request.Concept)
            }).ConfigureAwait(false);
        }
        await unitOfWork.CommitAsync().ConfigureAwait(false);

        return new CreateTransferResponse
        {
            TransactionIdentifier = externalId
        };
    }

    private static TransferBetweenAccountsRequest BuildBankCoreTransferRequest(
        CreateTransferRequest request,
        AccountUser account,
        CoreBeneficiary externalBeneficiary)
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
                TipoCuenta = MapBeneficiaryAccountTypeCode(externalBeneficiary.AccountType),
                Cuenta = externalBeneficiary.AccountNumber,
                Banco = externalBeneficiary.BankName ?? "Banco Bolivariano",
            },
            Beneficiario = new TransferBeneficiaryInfo
            {
                Nombre = externalBeneficiary.Name,
                Identificacion = new TransferIdentificationInfo
                {
                    Tipo = "C",
                    Numero = externalBeneficiary.Identification,
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
