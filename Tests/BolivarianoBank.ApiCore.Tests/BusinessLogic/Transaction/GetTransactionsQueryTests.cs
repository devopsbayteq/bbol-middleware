using LogicApi.BusinessLogic.Transaction;
using LogicApi.Model.Request.Transaction;
using LogicApi.Model.Response.Transaction;
using Microsoft.Extensions.Logging;
using Moq;
using PersistenceDb.Model;
using PersistenceDb.Models.Enums;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using PersistenceDb.Repository.Models;
using System.Linq.Expressions;

namespace BolivarianoBank.ApiCore.Tests.BusinessLogic.Transaction;

/// <summary>
/// Pruebas unitarias para GetTransactionsQueryHandler
/// </summary>
public class GetTransactionsQueryTests : BaseTests
{
    protected GetTransactionsQueryHandler Handler;
    protected Mock<IUnitOfWork> UnitOfWork;

    [SetUp]
    public async Task Setup()
    {
        Mock<ILogger<GetTransactionsQueryHandler>> logger = new();
        UnitOfWork = new();

        Handler = new GetTransactionsQueryHandler(logger.Object, UnitOfWork.Object);
    }

    [Test]
    public async Task TGTQ_01_GetTransactionsWithNoFilters()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            PageNumber = 1,
            PageSize = 10
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-001",
                BeneficiaryName = "John Doe",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Savings,
                BeneficiaryAccountNumber = "1234567890",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Checking,
                AccountNumber = "0987654321",
                AccountType = LogicApi.Model.Enums.AccountType.Savings,
                Concept = "Test transfer",
                Amount = -50m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now,
                BalanceAfterTransaction = 100m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 1);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.TotalCount, Is.EqualTo(1));
            Assert.That(response.PageNumber, Is.EqualTo(1));
            Assert.That(response.PageSize, Is.EqualTo(10));
            Assert.That(response.Items.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task TGTQ_02_GetTransactionsWithDateFilter()
    {
        var accountGuid = Guid.NewGuid();
        var dateFrom = DateTime.Now.AddDays(-30);
        var dateTo = DateTime.Now;

        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            DateFrom = dateFrom,
            DateTo = dateTo,
            PageNumber = 1,
            PageSize = 10
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-002",
                BeneficiaryName = "Jane Smith",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Checking,
                BeneficiaryAccountNumber = "5555555555",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Savings,
                AccountNumber = "4444444444",
                AccountType = LogicApi.Model.Enums.AccountType.Checking,
                Concept = "Monthly payment",
                Amount = -100m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now.AddDays(-15),
                BalanceAfterTransaction = 500m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 1);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.TotalCount, Is.EqualTo(1));
            Assert.That(response.Items.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task TGTQ_03_GetTransactionsWithTransactionTypeFilter()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            PageNumber = 1,
            PageSize = 10
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-003",
                BeneficiaryName = "Bob Johnson",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Savings,
                BeneficiaryAccountNumber = "7777777777",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Checking,
                AccountNumber = "8888888888",
                AccountType = LogicApi.Model.Enums.AccountType.Savings,
                Concept = "Transfer to Bob",
                Amount = -75m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now,
                BalanceAfterTransaction = 425m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 1);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Items.Count(), Is.EqualTo(1));
            Assert.That(response.Items.First().TransactionType, Is.EqualTo(LogicApi.Model.Enums.TransactionType.SentTransfers));
        });
    }

    [Test]
    public async Task TGTQ_04_GetTransactionsWithAmountRangeFilter()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            MinAmount = 50m,
            MaxAmount = 150m,
            PageNumber = 1,
            PageSize = 10
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-004",
                BeneficiaryName = "Alice Brown",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Checking,
                BeneficiaryAccountNumber = "3333333333",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Savings,
                AccountNumber = "2222222222",
                AccountType = LogicApi.Model.Enums.AccountType.Checking,
                Concept = "Payment",
                Amount = -100m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now,
                BalanceAfterTransaction = 325m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 1);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Items.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task TGTQ_05_GetTransactionsWithTextSearch()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            TextSearch = "payment",
            PageNumber = 1,
            PageSize = 10
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-005",
                BeneficiaryName = "Charlie Davis",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Savings,
                BeneficiaryAccountNumber = "6666666666",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Checking,
                AccountNumber = "9999999999",
                AccountType = LogicApi.Model.Enums.AccountType.Savings,
                Concept = "Monthly payment",
                Amount = -200m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now,
                BalanceAfterTransaction = 125m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 1);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Items.Count(), Is.EqualTo(1));
            Assert.That(response.Items.First().Concept, Does.Contain("payment").IgnoreCase);
        });
    }

    [Test]
    public async Task TGTQ_06_GetTransactionsWithMultipleFilters()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            DateFrom = DateTime.Now.AddDays(-30),
            DateTo = DateTime.Now,
            MinAmount = 50m,
            MaxAmount = 200m,
            TextSearch = "transfer",
            PageNumber = 1,
            PageSize = 10
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-006",
                BeneficiaryName = "Diana Evans",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Checking,
                BeneficiaryAccountNumber = "1111111111",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Savings,
                AccountNumber = "0000000000",
                AccountType = LogicApi.Model.Enums.AccountType.Checking,
                Concept = "Transfer to Diana",
                Amount = -150m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now.AddDays(-10),
                BalanceAfterTransaction = 275m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 1);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.TotalCount, Is.EqualTo(1));
            Assert.That(response.Items.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task TGTQ_07_GetTransactionsEmptyResult()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            PageNumber = 1,
            PageSize = 10
        };

        var paginatorResult = new PaginatorModel<TransactionItem>([], 0);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.TotalCount, Is.EqualTo(0));
            Assert.That(response.Items.Count(), Is.EqualTo(0));
        });
    }

    [Test]
    public async Task TGTQ_08_GetTransactionsPagination()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            PageNumber = 2,
            PageSize = 5
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-007",
                BeneficiaryName = "Frank Green",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Savings,
                BeneficiaryAccountNumber = "4567890123",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Checking,
                AccountNumber = "3456789012",
                AccountType = LogicApi.Model.Enums.AccountType.Savings,
                Concept = "Payment",
                Amount = -80m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now,
                BalanceAfterTransaction = 195m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 15);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.TotalCount, Is.EqualTo(15));
            Assert.That(response.PageNumber, Is.EqualTo(2));
            Assert.That(response.PageSize, Is.EqualTo(5));
        });
    }

    [Test]
    public async Task TGTQ_09_AccountNumbersMasked()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            PageNumber = 1,
            PageSize = 10
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-008",
                BeneficiaryName = "Grace Hill",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Checking,
                BeneficiaryAccountNumber = "1234567890",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Savings,
                AccountNumber = "0987654321",
                AccountType = LogicApi.Model.Enums.AccountType.Checking,
                Concept = "Test",
                Amount = -60m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now,
                BalanceAfterTransaction = 135m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 1);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Items.Count(), Is.EqualTo(1));
            var item = response.Items.First();
            Assert.That(item.BeneficiaryAccountNumber, Is.EqualTo("******7890"));
            Assert.That(item.AccountNumber, Is.EqualTo("******4321"));
        });
    }

    [Test]
    public async Task TGTQ_10_MultipleTransactionsReturned()
    {
        var accountGuid = Guid.NewGuid();
        var request = new GetTransactionsQueryRequest
        {
            AccountGuid = accountGuid,
            PageNumber = 1,
            PageSize = 20
        };

        var mockTransactions = new List<TransactionItem>
        {
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-009",
                BeneficiaryName = "Henry King",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Savings,
                BeneficiaryAccountNumber = "1111222233",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Checking,
                AccountNumber = "4444555566",
                AccountType = LogicApi.Model.Enums.AccountType.Savings,
                Concept = "First transfer",
                Amount = -30m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now.AddDays(-2),
                BalanceAfterTransaction = 170m
            },
            new ()
            {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-010",
                BeneficiaryName = "Ivy Lee",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Checking,
                BeneficiaryAccountNumber = "7777888899",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Savings,
                AccountNumber = "0000111122",
                AccountType = LogicApi.Model.Enums.AccountType.Checking,
                Concept = "Second transfer",
                Amount = -45m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now.AddDays(-1),
                BalanceAfterTransaction = 125m
            },
            new() {
                TransactionGuid = Guid.NewGuid(),
                TransactionIdentifier = "TXN-011",
                BeneficiaryName = "Jack Miller",
                BeneficiaryAccountType = LogicApi.Model.Enums.AccountType.Savings,
                BeneficiaryAccountNumber = "3333444455",
                OwnerAccountType = LogicApi.Model.Enums.AccountType.Checking,
                AccountNumber = "6666777788",
                AccountType = LogicApi.Model.Enums.AccountType.Savings,
                Concept = "Third transfer",
                Amount = -25m,
                TransactionType = LogicApi.Model.Enums.TransactionType.SentTransfers,
                TransferDate = DateTime.Now,
                BalanceAfterTransaction = 100m
            }
        };

        var paginatorResult = new PaginatorModel<TransactionItem>(mockTransactions, 3);

        UnitOfWork.Setup(u => u.TransactionRepository.GetPaginatorGenericAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, TransactionItem>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, bool>>>(),
            It.IsAny<Expression<Func<PersistenceDb.Models.Core.Transaction, object>>>(),
            It.IsAny<OrderByType>()))
            .Returns(Task.FromResult<IPaginator<TransactionItem>>(paginatorResult));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.TotalCount, Is.EqualTo(3));
            Assert.That(response.Items.Count(), Is.EqualTo(3));
            Assert.That(response.Items.All(i => i.AccountNumber.StartsWith("******")), Is.True);
            Assert.That(response.Items.All(i => i.BeneficiaryAccountNumber.StartsWith("******")), Is.True);
        });
    }
}
