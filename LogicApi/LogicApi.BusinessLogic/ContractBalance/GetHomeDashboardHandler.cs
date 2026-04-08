using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using LogicApi.Model.Enums;
using LogicApi.Model.Request.Beneficiary;
using LogicApi.Model.Request.ContractBalance;
using LogicApi.Model.Request.Transaction;
using LogicApi.Model.Response.ContractBalance;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using BannerItem = LogicApi.Model.Response.ContractBalance.BannerItem;

namespace LogicApi.BusinessLogic.ContractBalance;

/// <summary>
/// Handler para datos del home de productos
/// </summary>
public class GetHomeDashboardHandler(
    ILogger<GetHomeDashboardHandler> logger,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IOptions<AppSetting> options
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
        var icons = options.Value.HomeDashboardIcons;
        var accountTypeIcons = options.Value.AccountTypeIcons;
        var creditCardTypeIcons = options.Value.CreditCardTypeIcons;
        var loanTypeIcons = options.Value.LoanTypeIcons;
        var investmentTypeIcons = options.Value.InvestmentTypeIcons;
        var frequentPaymentTypeIcons = options.Value.FrequentPaymentTypeIcons;
        var beneficiaryAccountsDictionary = beneficiaryAccounts.Contacts.ToDictionary(x => x.BeneficiaryAccountNumber);

        var recentTransactions = await mediator.Send(new GetTransactionsQueryRequest
        {
            ContextRequest = request.ContextRequest,
            PageSize = 3,
            PageNumber = 1,
            AccountGuid = userAccounts.FirstOrDefault()?.Guid ?? throw new CustomException(MessageCodes.UserContextNotFound, "No se pudo resolver la cuenta del usuario."),
        }, cancellationToken).ConfigureAwait(false);

        var listAccountAlias = new[]{
            "Gastos",
            "Departamento",
            "Emergencias",
            "Universidad",
            "Cuentas bancarias",
        };

        var response = new GetHomeDashboardResponse
        {
            RecentTransactions = [.. recentTransactions.Items],
            HomeDashboardIcons = icons,
            Banners = [.. options.Value.HomeDashboardBanners.Select(x => new BannerItem()
            {
                Text = x.Text,
                ButtonText = x.ButtonText,
                ButtonLink = x.ButtonLink,
                Landscape = x.Landscape
            })],
            Accounts = [.. userAccounts.Select((account, index) => new HomeAccountItem
                {
                    AccountTypeIcons = accountTypeIcons,
                    AccountGuid = account.Guid,
                    MaskedAccountNumber = account.AccountNumber,
                    AccountType = (AccountType)account.AccountType,
                    Balance = balanceByAccount.TryGetValue(account.Guid, out var balance) ? balance : 0m,
                    Beneficiary = beneficiaryAccountsDictionary.FirstOrDefaultValue(account.AccountNumber),
                    AccountAlias = listAccountAlias[index]
                })],
            CreditCards =
            [
                new()
                {
                    CreditCardTypeIcons = creditCardTypeIcons,
                    MaskedCardNumber = "**** **** **** 1245",
                    TotalDue = 280.73m,
                    MaxPaymentDate = DateTime.UtcNow.Date.AddDays(12)
                }
            ],
            Loans =
            [
                new()
                {
                    LoanTypeIcons = loanTypeIcons,
                    LoanGuid = "***** 678",
                    OutstandingBalance = 7800.10m,
                    NextInstallmentAmount = 230.50m,
                    NextInstallmentDate = DateTime.UtcNow.Date.AddDays(15)
                }
            ],
            Investments =
            [
                new()
                {
                    InvestmentTypeIcons = investmentTypeIcons,
                    InvestmentGuid = "137********",
                    ProductName = "Desposito a plazo fijo",
                    CurrentValue = 3540.90m,
                    Currency = "USD"
                }
            ],
            FrequentPayments = frequentPaymentTypeIcons.Select(x => new HomeFrequentPaymentItem
            {
                BeneficiaryName = x.Text,
                BeneficiaryType = x.IconCode
            }).ToList()

        };

        response.TotalBalance = response.Accounts.Sum(x => x.Balance);

        return response;
    }

}
