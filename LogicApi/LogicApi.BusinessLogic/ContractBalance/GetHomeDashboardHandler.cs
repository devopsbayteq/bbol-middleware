using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using LogicApi.Model.Enums;
using LogicApi.Model.Request.Beneficiary;
using LogicApi.Model.Request.ContractBalance;
using LogicApi.Model.Response.ContractBalance;
using MediatR;
using Microsoft.Extensions.Logging;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace LogicApi.BusinessLogic.ContractBalance;

/// <summary>
/// Handler para datos del home de productos
/// </summary>
public class GetHomeDashboardHandler(
    ILogger<GetHomeDashboardHandler> logger,
    IUnitOfWork unitOfWork,
    IMediator mediator
    ) : ContractBalanceBase<GetHomeDashboardRequest, GetHomeDashboardResponse>(logger)
{
    public override async Task<GetHomeDashboardResponse> Handle(GetHomeDashboardRequest request, CancellationToken cancellationToken)
    {
        var userGuid = request.ContextRequest?.CustomClaims?.UserId ?? throw new CustomException(MessageCodes.UserContextNotFound, "No se pudo resolver el usuario del contexto.");

        var userAccounts = await unitOfWork.AccountUserRepository
            .GetByAsync(a => a.UserId == userGuid)
            .ConfigureAwait(false);

        var accountIds = userAccounts.Select(a => a.Guid).ToList();
        var balanceByAccount = await unitOfWork.TransactionRepository
            .GetAmountByAccountsAsync(accountIds)
            .ConfigureAwait(false);
        var beneficiaryAccounts = await mediator.Send(new GetBeneficiaryContactsRequest
        {
            BeneficiaryType = BeneficiaryType.OwnAccounts,
            ContextRequest = request.ContextRequest
        }, cancellationToken).ConfigureAwait(false);
        var beneficiaryAccountsDictionary = beneficiaryAccounts.Contacts.ToDictionary(x => x.BeneficiaryAccountNumber);
        var response = new GetHomeDashboardResponse
        {
            Accounts = [.. userAccounts.Select(account => new HomeAccountItem
                {
                    AccountGuid = account.Guid,
                    MaskedAccountNumber = account.AccountNumber,
                    AccountType = (AccountType)account.AccountType,
                    Balance = balanceByAccount.TryGetValue(account.Guid, out var balance) ? balance : 0m,
                    Beneficiary = beneficiaryAccountsDictionary.FirstOrDefaultValue(account.AccountNumber)
                })],
            CreditCards =
            [
                new()
                {
                    MaskedCardNumber = "**** **** **** 1245",
                    TotalDue = 280.73m,
                    MaxPaymentDate = DateTime.UtcNow.Date.AddDays(12)
                }
            ],
            Loans =
            [
                new()
                {
                    LoanGuid = Guid.NewGuid().ToString(),
                    OutstandingBalance = 7800.10m,
                    NextInstallmentAmount = 230.50m,
                    NextInstallmentDate = DateTime.UtcNow.Date.AddDays(15)
                }
            ],
            Investments =
            [
                new()
                {
                    InvestmentGuid = Guid.NewGuid().ToString(),
                    ProductName = "Fondo Conservador",
                    CurrentValue = 3540.90m,
                    Currency = "USD"
                }
            ],
            FrequentPayments =
            [
                new()
                {
                    BeneficiaryName = "CNEL",
                    BeneficiaryType = "BasicService"
                },
                new()
                {
                    BeneficiaryName = "Carlos Herrera",
                    BeneficiaryType = "Account"
                }
            ]
        };

        response.TotalBalance = response.Accounts.Sum(x => x.Balance);

        return response;
    }

}
