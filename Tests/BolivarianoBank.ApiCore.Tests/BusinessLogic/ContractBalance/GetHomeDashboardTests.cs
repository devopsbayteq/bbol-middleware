using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using Common.WebApi.Models;
using Common.WebApi.Models.AppSettings;
using LogicApi.BusinessLogic.ContractBalance;
using LogicApi.Model.Request.Beneficiary;
using LogicApi.Model.Request.ContractBalance;
using LogicApi.Model.Request.Transaction;
using LogicApi.Model.Response.Beneficiary;
using LogicApi.Model.Response.Transaction;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PersistenceDb.Models.Core;
using PersistenceDb.Models.Enums;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using System.Linq.Expressions;

namespace BolivarianoBank.ApiCore.Tests.BusinessLogic.ContractBalance;

/// <summary>
/// Pruebas unitarias para GetHomeDashboardHandler
/// </summary>
public class GetHomeDashboardTests : BaseTests
{
    protected GetHomeDashboardHandler Handler;
    protected Mock<IUnitOfWork> UnitOfWork;
    protected Mock<IMediator> Mediator;
    protected Mock<IOptions<AppSetting>> Options;

    [SetUp]
    public async Task Setup()
    {
        Mock<ILogger<GetHomeDashboardHandler>> logger = new();
        UnitOfWork = new();
        Mediator = new();
        Options = new();

        // Setup default AppSetting options
        var appSettings = new AppSetting
        {
            HomeDashboardIcons = [],
            AccountTypeIcons = [],
            CreditCardTypeIcons = [],
            LoanTypeIcons = [],
            InvestmentTypeIcons = [],
            FrequentPaymentTypeIcons = [
            
                new() { Text = "Payment 1", IconCode = "P1" },
                new() { Text = "Payment 2", IconCode = "P2" }
            ],
            HomeDashboardBanners = []
        };

        Options.Setup(o => o.Value).Returns(appSettings);

        // Setup default mediator responses
        Mediator.Setup(m => m.Send(It.IsAny<GetBeneficiaryContactsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetBeneficiaryContactsResponse { Contacts = [] });

        Mediator.Setup(m => m.Send(It.IsAny<GetTransactionsQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTransactionsQueryResponse { Items = new List<TransactionItem>() });

        Handler = new GetHomeDashboardHandler(logger.Object, UnitOfWork.Object, Mediator.Object, Options.Object);
    }

    [Test]
    public async Task TGHD_01_UserContextNotFound()
    {
        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest()
        };

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.UserContextNotFound));
            Assert.That(ex.Message, Does.Contain("No se pudo resolver el usuario del contexto"));
        });
    }

    [Test]
    public async Task TGHD_02_UserWithNoAccounts()
    {
        var userGuid = Guid.NewGuid();
        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>([]));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(new Dictionary<Guid, decimal>()));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.UserContextNotFound));
            Assert.That(ex.Message, Does.Contain("No se pudo resolver la cuenta del usuario"));
        });
    }

    [Test]
    public async Task TGHD_03_UserWithSingleSavingsAccount()
    {
        var userGuid = Guid.NewGuid();
        var accountGuid = Guid.NewGuid();
        var accountBalance = 1500.50m;

        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        var accounts = new List<AccountUser>
        {
            new ()
            {
                Guid = accountGuid,
                AccountNumber = "1234567890",
                AccountType = AccountType.Savings,
                UserId = userGuid
            }
        };

        var balances = new Dictionary<Guid, decimal>
        {
            { accountGuid, accountBalance }
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>(accounts));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(balances));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Accounts, Has.Count.EqualTo(1));
            Assert.That(response.Accounts[0].AccountGuid, Is.EqualTo(accountGuid));
            Assert.That(response.Accounts[0].MaskedAccountNumber, Is.EqualTo("******7890"));
            Assert.That(response.Accounts[0].AccountType, Is.EqualTo(LogicApi.Model.Enums.AccountType.Savings));
            Assert.That(response.Accounts[0].Balance, Is.EqualTo(accountBalance));
            Assert.That(response.TotalBalance, Is.EqualTo(accountBalance));
        });
    }

    [Test]
    public async Task TGHD_04_UserWithSingleCheckingAccount()
    {
        var userGuid = Guid.NewGuid();
        var accountGuid = Guid.NewGuid();
        var accountBalance = 2750.75m;

        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        var accounts = new List<AccountUser>
        {
            new ()
            {
                Guid = accountGuid,
                AccountNumber = "9876543210",
                AccountType = AccountType.Checking,
                UserId = userGuid
            }
        };

        var balances = new Dictionary<Guid, decimal>
        {
            { accountGuid, accountBalance }
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>(accounts));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(balances));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Accounts, Has.Count.EqualTo(1));
            Assert.That(response.Accounts[0].AccountType, Is.EqualTo(LogicApi.Model.Enums.AccountType.Checking));
            Assert.That(response.Accounts[0].Balance, Is.EqualTo(accountBalance));
            Assert.That(response.TotalBalance, Is.EqualTo(accountBalance));
        });
    }

    [Test]
    public async Task TGHD_05_UserWithMultipleAccounts()
    {
        var userGuid = Guid.NewGuid();
        var account1Guid = Guid.NewGuid();
        var account2Guid = Guid.NewGuid();
        var account3Guid = Guid.NewGuid();
        var balance1 = 1000m;
        var balance2 = 2500m;
        var balance3 = 500m;

        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        var accounts = new List<AccountUser>
        {
            new ()
            {
                Guid = account1Guid,
                AccountNumber = "1111111111",
                AccountType = AccountType.Savings,
                UserId = userGuid
            },
            new ()
            {
                Guid = account2Guid,
                AccountNumber = "2222222222",
                AccountType = AccountType.Checking,
                UserId = userGuid
            },
            new ()
            {
                Guid = account3Guid,
                AccountNumber = "3333333333",
                AccountType = AccountType.Savings,
                UserId = userGuid
            }
        };

        var balances = new Dictionary<Guid, decimal>
        {
            { account1Guid, balance1 },
            { account2Guid, balance2 },
            { account3Guid, balance3 }
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>(accounts));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(balances));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Accounts, Has.Count.EqualTo(3));
            Assert.That(response.TotalBalance, Is.EqualTo(balance1 + balance2 + balance3));
            Assert.That(response.Accounts[0].MaskedAccountNumber, Is.EqualTo("******1111"));
            Assert.That(response.Accounts[1].MaskedAccountNumber, Is.EqualTo("******2222"));
            Assert.That(response.Accounts[2].MaskedAccountNumber, Is.EqualTo("******3333"));
        });
    }

    [Test]
    public async Task TGHD_06_AccountWithZeroBalance()
    {
        var userGuid = Guid.NewGuid();
        var accountGuid = Guid.NewGuid();

        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        var accounts = new List<AccountUser>
        {
            new ()
            {
                Guid = accountGuid,
                AccountNumber = "5555555555",
                AccountType = AccountType.Savings,
                UserId = userGuid
            }
        };

        var balances = new Dictionary<Guid, decimal>
        {
            { accountGuid, 0m }
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>(accounts));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(balances));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Accounts[0].Balance, Is.EqualTo(0m));
            Assert.That(response.TotalBalance, Is.EqualTo(0m));
        });
    }

    [Test]
    public async Task TGHD_07_AccountWithNegativeBalance()
    {
        var userGuid = Guid.NewGuid();
        var accountGuid = Guid.NewGuid();
        var negativeBalance = -250.50m;

        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        var accounts = new List<AccountUser>
        {
            new()
            {
                Guid = accountGuid,
                AccountNumber = "6666666666",
                AccountType = AccountType.Checking,
                UserId = userGuid
            }
        };

        var balances = new Dictionary<Guid, decimal>
        {
            { accountGuid, negativeBalance }
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>(accounts));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(balances));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Accounts[0].Balance, Is.EqualTo(negativeBalance));
            Assert.That(response.TotalBalance, Is.EqualTo(negativeBalance));
        });
    }

    [Test]
    public async Task TGHD_08_AccountWithoutBalanceInDictionary()
    {
        var userGuid = Guid.NewGuid();
        var accountGuid = Guid.NewGuid();

        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        var accounts = new List<AccountUser>
        {
            new ()
            {
                Guid = accountGuid,
                AccountNumber = "7777777777",
                AccountType = AccountType.Savings,
                UserId = userGuid
            }
        };

        // Balance dictionary doesn't contain the account
        var balances = new Dictionary<Guid, decimal>();

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>(accounts));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(balances));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Accounts[0].Balance, Is.EqualTo(0m));
            Assert.That(response.TotalBalance, Is.EqualTo(0m));
        });
    }

    [Test]
    public async Task TGHD_09_ResponseContainsAllProductTypes()
    {
        var userGuid = Guid.NewGuid();
        var accountGuid = Guid.NewGuid();

        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        var accounts = new List<AccountUser>
        {
            new ()
            {
                Guid = accountGuid,
                AccountNumber = "8888888888",
                AccountType = AccountType.Savings,
                UserId = userGuid
            }
        };

        var balances = new Dictionary<Guid, decimal>
        {
            { accountGuid, 1000m }
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>(accounts));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(balances));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Accounts, Is.Not.Empty);
            Assert.That(response.CreditCards, Is.Not.Empty);
            Assert.That(response.CreditCards, Has.Count.EqualTo(1));
            Assert.That(response.CreditCards[0].MaskedCardNumber, Is.EqualTo("**** **** **** 1245"));
            Assert.That(response.Loans, Is.Not.Empty);
            Assert.That(response.Loans, Has.Count.EqualTo(1));
            Assert.That(response.Investments, Is.Not.Empty);
            Assert.That(response.Investments, Has.Count.EqualTo(1));
            Assert.That(response.FrequentPayments, Is.Not.Empty);
            Assert.That(response.FrequentPayments, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public async Task TGHD_10_MixedAccountBalances()
    {
        var userGuid = Guid.NewGuid();
        var account1Guid = Guid.NewGuid();
        var account2Guid = Guid.NewGuid();
        var account3Guid = Guid.NewGuid();
        var balance1 = 5000m;
        var balance2 = -100m;
        var balance3 = 0m;

        var request = new GetHomeDashboardRequest
        {
            ContextRequest = new ContextRequest
            {
                CustomClaims = new()
                {
                    UserId = userGuid
                }
            }
        };

        var accounts = new List<AccountUser>
        {
            new ()
            {
                Guid = account1Guid,
                AccountNumber = "1234567890",
                AccountType = AccountType.Savings,
                UserId = userGuid
            },
            new ()
            {
                Guid = account2Guid,
                AccountNumber = "0987654321",
                AccountType = AccountType.Checking,
                UserId = userGuid
            },
            new ()
            {
                Guid = account3Guid,
                AccountNumber = "5555666677",
                AccountType = AccountType.Savings,
                UserId = userGuid
            }
        };

        var balances = new Dictionary<Guid, decimal>
        {
            { account1Guid, balance1 },
            { account2Guid, balance2 },
            { account3Guid, balance3 }
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<List<AccountUser>>(accounts));

        UnitOfWork.Setup(u => u.TransactionRepository.GetAmountByAccountsAsync(It.IsAny<List<Guid>>()))
            .Returns(Task.FromResult(balances));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Accounts, Has.Count.EqualTo(3));
            Assert.That(response.TotalBalance, Is.EqualTo(balance1 + balance2 + balance3));
            Assert.That(response.TotalBalance, Is.EqualTo(4900m));
            Assert.That(response.Accounts[0].Balance, Is.EqualTo(balance1));
            Assert.That(response.Accounts[1].Balance, Is.EqualTo(balance2));
            Assert.That(response.Accounts[2].Balance, Is.EqualTo(balance3));
        });
    }
}
