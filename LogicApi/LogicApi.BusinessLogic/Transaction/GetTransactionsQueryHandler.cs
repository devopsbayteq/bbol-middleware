using System.Text;
using Common.Utils.Extensions;
using LogicApi.Model.Request.Transaction;
using LogicApi.Model.Response.Transaction;
using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Enums;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace LogicApi.BusinessLogic.Transaction;

/// <summary>
/// Handler para consulta paginada de transacciones
/// </summary>
public class GetTransactionsQueryHandler(
    ILogger<GetTransactionsQueryHandler> logger,
    IUnitOfWork unitOfWork
    ) : TransactionBase<GetTransactionsQueryRequest, GetTransactionsQueryResponse>(logger)
{
    /// <summary>
    /// Handle para consulta paginada de transacciones
    /// </summary>
    /// <param name="request">Solicitud de consulta paginada de transacciones</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta paginada de transacciones</returns>
    public override async Task<GetTransactionsQueryResponse> Handle(GetTransactionsQueryRequest request, CancellationToken cancellationToken)
    {
        var dateFrom = request.DateFrom?.Date;
        var dateTo = request.DateTo?.AddDays(1).Date;
        var searchTextNormalized = NormalizeText(request.TextSearch);
        var transactionTypes = request.TransactionTypes?.Select(x => (TransactionType)x).ToList();
        // Si request.TransactionTypes es null, genera el listado de todos los TransactionType
        if (request.TransactionTypes.IsNullOrEmpty())
            transactionTypes = [.. Enum.GetValues<TransactionType>()];


        var transactions = await unitOfWork.TransactionRepository.GetPaginatorGenericAsync(
            request.PageSize,
            request.PageNumber,
            select => new TransactionItem
            {
                TransactionGuid = select.Id,
                TransactionIdentifier = select.ExternalIdentifier,
                BeneficiaryName = select.Beneficiary.Name,
                BeneficiaryAccountType = (Model.Enums.AccountType)select.Beneficiary.AccountType,
                BeneficiaryAccountNumber = select.Beneficiary.AccountNumber,
                OwnerAccountType = (Model.Enums.AccountType)select.Account.AccountType,
                AccountNumber = select.Account.AccountNumber,
                AccountType = (Model.Enums.AccountType)select.Account.AccountType,
                Concept = select.Description,
                Amount = select.Amount,
                TransactionType = (Model.Enums.TransactionType)select.TransactionType,
                TransferDate = select.RegisterDate,
                BalanceAfterTransaction = select.ResultingAmount,
            },
            where => where.AccountGuid == request.AccountGuid
                && (!dateFrom.HasValue || where.RegisterDate >= dateFrom.Value)
                && (!dateTo.HasValue || where.RegisterDate <= dateTo.Value)
                && transactionTypes.Contains((TransactionType)where.TransactionType)
                && (!request.MaxAmount.HasValue || where.AbsoluteAmount <= request.MaxAmount.Value)
                && (!request.MinAmount.HasValue || where.AbsoluteAmount >= request.MinAmount.Value)
                && (string.IsNullOrEmpty(searchTextNormalized) || where.NormalizedDescription.Contains(searchTextNormalized)),
                orderBy => orderBy.RegisterDate,
                OrderByType.Desc
        ).ConfigureAwait(false);

        return new GetTransactionsQueryResponse
        {
            TotalCount = transactions.TotalItems,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Items = transactions.Items.Select(item =>
            {
                item.BeneficiaryAccountNumber = MaskAccountNumber(item.BeneficiaryAccountNumber);
                item.AccountNumber = MaskAccountNumber(item.AccountNumber);
                return item;
            })
        };
    }





}
