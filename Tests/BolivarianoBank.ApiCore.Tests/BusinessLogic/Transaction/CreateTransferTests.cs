using BankCore.Integration.Interfaces;
using BankCore.Integration.Models.Transfer;
using Common.WebApi.Clock;
using Common.WebApi.Exceptions;
using Common.WebApi.Messages;
using LogicApi.BusinessLogic.Transaction;
using LogicApi.Model.Request.Transaction;
using Microsoft.Extensions.Logging;
using Moq;
using PersistenceDb.Models.Core;
using PersistenceDb.Models.Enums;
using PersistenceDb.Repository.Interfaces.UnitOfWork;
using System.Linq.Expressions;
using CoreTransaction = PersistenceDb.Models.Core.Transaction;

namespace BolivarianoBank.ApiCore.Tests.BusinessLogic.Transaction;

/// <summary>
/// Pruebas unitarias para CreateTransferHandler
/// </summary>
public class CreateTransferTests : BaseTests
{
    protected CreateTransferHandler Handler;
    protected Mock<IUnitOfWork> UnitOfWork;
    protected Mock<IClock> Clock;
    protected Mock<IBankCoreServices> BankCoreServices;

    [SetUp]
    public async Task Setup()
    {
        Mock<ILogger<CreateTransferHandler>> logger = new();
        UnitOfWork = new();
        Clock = new();
        BankCoreServices = new();

        Clock.Setup(c => c.Now()).Returns(DateTime.Now);
        Clock.Setup(c => c.UtcNow()).Returns(DateTime.UtcNow);

        Handler = new CreateTransferHandler(
            logger.Object,
            UnitOfWork.Object,
            Clock.Object,
            BankCoreServices.Object);
    }

    [Test]
    public async Task TCT_01_AccountNotExists()
    {
        var request = new CreateTransferRequest
        {
            Amount = 50m,
            AccountGuid = Guid.NewGuid(),
            BeneficiaryContactGuid = Guid.NewGuid(),
            Concept = "Test transfer"
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<AccountUser>(null!));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.SystemError));
            Assert.That(ex.Message, Does.Contain("La cuenta origen no existe"));
        });
    }

    [Test]
    public async Task TCT_02_InsufficientBalance()
    {
        var accountGuid = Guid.NewGuid();
        var beneficiaryGuid = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new CreateTransferRequest
        {
            Amount = 50m,
            AccountGuid = accountGuid,
            BeneficiaryContactGuid = beneficiaryGuid,
            Concept = "Test transfer"
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<AccountUser>(new AccountUser
            {
                Guid = accountGuid,
                AccountNumber = "1234567890",
                AccountType = AccountType.Savings,
                UserId = userId
            }));

        UnitOfWork.Setup(u => u.BeneficiaryRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>()))
            .Returns(Task.FromResult<Beneficiary>(new Beneficiary
            {
                Id = beneficiaryGuid,
                Name = "John Doe",
                Identification = "1234567890",
                AccountNumber = "0987654321",
                AccountType = 1,
                BankName = "Test Bank"
            }));

        // Balance actual es 30, pero se intenta transferir 50
        UnitOfWork.Setup(u => u.TransactionRepository.SumAsync(
            It.IsAny<Expression<Func<CoreTransaction, decimal>>>(),
            It.IsAny<Expression<Func<CoreTransaction, bool>>>()))
            .Returns(Task.FromResult(30m));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.TransactionAmountInsufficient));
            Assert.That(ex.Message, Does.Contain("El saldo: 30 de la cuenta es insuficiente"));
        });
    }

    [Test]
    public async Task TCT_03_BankCoreServiceError()
    {
        var accountGuid = Guid.NewGuid();
        var beneficiaryGuid = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new CreateTransferRequest
        {
            Amount = 50m,
            AccountGuid = accountGuid,
            BeneficiaryContactGuid = beneficiaryGuid,
            Concept = "Test transfer"
        };

        UnitOfWork.Setup(u => u.TransactionRepository.SumAsync(
            It.IsAny<Expression<Func<CoreTransaction, decimal>>>(),
            It.IsAny<Expression<Func<CoreTransaction, bool>>>()))
            .Returns(Task.FromResult(100m));

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<AccountUser>(new AccountUser
            {
                Guid = accountGuid,
                AccountNumber = "1234567890",
                AccountType = AccountType.Savings,
                UserId = userId
            }));

        UnitOfWork.Setup(u => u.BeneficiaryRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>()))
            .Returns(Task.FromResult<Beneficiary>(new Beneficiary
            {
                Id = beneficiaryGuid,
                Name = "John Doe",
                Identification = "1234567890",
                AccountNumber = "0987654321",
                AccountType = 1,
                BankName = "Test Bank"
            }));

        BankCoreServices.Setup(b => b.TransferBetweenAccountsAsync(
            It.IsAny<TransferBetweenAccountsRequest>(),
            It.IsAny<CancellationToken>()))
            .Throws(new BankCore.Integration.Exceptions.BankCoreUserMessageException("Error en el core bancario"));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.BankCoreUserMessage));
            Assert.That(ex.Message, Does.Contain("Error en el core bancario"));
        });
    }

    [Test]
    public async Task TCT_04_BankCoreResponseNull()
    {
        var accountGuid = Guid.NewGuid();
        var beneficiaryGuid = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new CreateTransferRequest
        {
            Amount = 50m,
            AccountGuid = accountGuid,
            BeneficiaryContactGuid = beneficiaryGuid,
            Concept = "Test transfer"
        };

        UnitOfWork.Setup(u => u.TransactionRepository.SumAsync(
            It.IsAny<Expression<Func<CoreTransaction, decimal>>>(),
            It.IsAny<Expression<Func<CoreTransaction, bool>>>()))
            .Returns(Task.FromResult(100m));

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<AccountUser>(new AccountUser
            {
                Guid = accountGuid,
                AccountNumber = "1234567890",
                AccountType = AccountType.Savings,
                UserId = userId
            }));

        UnitOfWork.Setup(u => u.BeneficiaryRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>()))
            .Returns(Task.FromResult<Beneficiary>(new Beneficiary
            {
                Id = beneficiaryGuid,
                Name = "John Doe",
                Identification = "1234567890",
                AccountNumber = "0987654321",
                AccountType = 1,
                BankName = "Test Bank"
            }));

        BankCoreServices.Setup(b => b.TransferBetweenAccountsAsync(
            It.IsAny<TransferBetweenAccountsRequest>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<TransferBetweenAccountsResponse>(null!));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.SystemError));
            Assert.That(ex.Message, Does.Contain("El core bancario no devolvió un identificador de transacción válido"));
        });
    }

    [Test]
    public async Task TCT_05_BankCoreResponseWithEmptyTransactionId()
    {
        var accountGuid = Guid.NewGuid();
        var beneficiaryGuid = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new CreateTransferRequest
        {
            Amount = 50m,
            AccountGuid = accountGuid,
            BeneficiaryContactGuid = beneficiaryGuid,
            Concept = "Test transfer"
        };

        UnitOfWork.Setup(u => u.TransactionRepository.SumAsync(
            It.IsAny<Expression<Func<CoreTransaction, decimal>>>(),
            It.IsAny<Expression<Func<CoreTransaction, bool>>>()))
            .Returns(Task.FromResult(100m));

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<AccountUser>(new AccountUser
            {
                Guid = accountGuid,
                AccountNumber = "1234567890",
                AccountType = AccountType.Savings,
                UserId = userId
            }));

        UnitOfWork.Setup(u => u.BeneficiaryRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>()))
            .Returns(Task.FromResult<Beneficiary>(new Beneficiary
            {
                Id = beneficiaryGuid,
                Name = "John Doe",
                Identification = "1234567890",
                AccountNumber = "0987654321",
                AccountType = 1,
                BankName = "Test Bank"
            }));

        BankCoreServices.Setup(b => b.TransferBetweenAccountsAsync(
            It.IsAny<TransferBetweenAccountsRequest>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new TransferBetweenAccountsResponse
            {
                SecuenciaTransaccion = string.Empty
            }));

        var ex = Assert.ThrowsAsync<CustomException>(async () => await Handler.Handle(request, It.IsAny<CancellationToken>()));

        Assert.Multiple(() =>
        {
            Assert.That(ex.MessageCode, Is.EqualTo(MessageCodes.SystemError));
            Assert.That(ex.Message, Does.Contain("El core bancario no devolvió un identificador de transacción válido"));
        });
    }

    [Test]
    public async Task TCT_06_CreateTransferSuccessful()
    {
        var accountGuid = Guid.NewGuid();
        var beneficiaryGuid = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transactionId = "TXN-123456789";
        var request = new CreateTransferRequest
        {
            Amount = 50m,
            AccountGuid = accountGuid,
            BeneficiaryContactGuid = beneficiaryGuid,
            Concept = "Test transfer"
        };

        UnitOfWork.Setup(u => u.TransactionRepository.SumAsync(
            It.IsAny<Expression<Func<CoreTransaction, decimal>>>(),
            It.IsAny<Expression<Func<CoreTransaction, bool>>>()))
            .Returns(Task.FromResult(100m));

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<AccountUser>(new AccountUser
            {
                Guid = accountGuid,
                AccountNumber = "1234567890",
                AccountType = AccountType.Savings,
                UserId = userId
            }));

        UnitOfWork.Setup(u => u.BeneficiaryRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>()))
            .Returns(Task.FromResult<Beneficiary>(new Beneficiary
            {
                Id = beneficiaryGuid,
                Name = "John Doe",
                Identification = "1234567890",
                AccountNumber = "0987654321",
                AccountType = 1,
                BankName = "Test Bank"
            }));

        BankCoreServices.Setup(b => b.TransferBetweenAccountsAsync(
            It.IsAny<TransferBetweenAccountsRequest>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new TransferBetweenAccountsResponse
            {
                SecuenciaTransaccion = transactionId
            }));

        UnitOfWork.Setup(u => u.TransactionRepository.AddAsync(It.IsAny<CoreTransaction>()))
            .Returns(Task.FromResult(new CoreTransaction
            {
                Id = Guid.NewGuid(),
                ExternalIdentifier = transactionId
            }));

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByFirstOrDefaultAsync(It.Is<Expression<Func<AccountUser, bool>>>(
            expr => expr.ToString().Contains("AccountNumber"))))
            .Returns(Task.FromResult<AccountUser>(null!));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.TransactionIdentifier, Is.EqualTo(transactionId));
        });

        // Verify that AddAsync was called to save the transaction
        UnitOfWork.Verify(u => u.TransactionRepository.AddAsync(It.Is<CoreTransaction>(
            t => t.AccountGuid == accountGuid &&
                 t.BeneficiaryGuid == beneficiaryGuid &&
                 t.Amount == -50m &&
                 t.AbsoluteAmount == 50m &&
                 t.ExternalIdentifier == transactionId &&
                 t.TransactionType == (byte)TransactionType.SentTransfers
        )), Times.Once);
    }

    [Test]
    public async Task TCT_13_CreateTransferWithCheckingAccount()
    {
        var accountGuid = Guid.NewGuid();
        var beneficiaryGuid = Guid.NewGuid();
        var transactionId = "TXN-987654321";
        var request = new CreateTransferRequest
        {
            Amount = 75m,
            AccountGuid = accountGuid,
            BeneficiaryContactGuid = beneficiaryGuid,
            Concept = "Transfer from checking account"
        };

        UnitOfWork.Setup(u => u.AccountUserRepository.ExistAnyAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult(true));

        UnitOfWork.Setup(u => u.BeneficiaryRepository.ExistAnyAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>()))
            .Returns(Task.FromResult(true));

        UnitOfWork.Setup(u => u.TransactionRepository.SumAsync(
            It.IsAny<Expression<Func<CoreTransaction, decimal>>>(),
            It.IsAny<Expression<Func<CoreTransaction, bool>>>()))
            .Returns(Task.FromResult(200m));

        UnitOfWork.Setup(u => u.AccountUserRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<AccountUser, bool>>>()))
            .Returns(Task.FromResult<AccountUser>(new AccountUser
            {
                Guid = accountGuid,
                AccountNumber = "9876543210",
                AccountType = AccountType.Checking
            }));

        UnitOfWork.Setup(u => u.BeneficiaryRepository.GetByFirstOrDefaultAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>()))
            .Returns(Task.FromResult<Beneficiary>(new Beneficiary
            {
                Id = beneficiaryGuid,
                Name = "Jane Smith",
                Identification = "0987654321",
                AccountNumber = "1122334455",
                AccountType = 2,
                BankName = "Another Bank"
            }));

        BankCoreServices.Setup(b => b.TransferBetweenAccountsAsync(
            It.IsAny<TransferBetweenAccountsRequest>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new TransferBetweenAccountsResponse
            {
                SecuenciaTransaccion = transactionId
            }));

        UnitOfWork.Setup(u => u.TransactionRepository.AddAsync(It.IsAny<CoreTransaction>()))
            .Returns(Task.FromResult(new CoreTransaction
            {
                Id = Guid.NewGuid(),
                ExternalIdentifier = transactionId
            }));

        var response = await Handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.TransactionIdentifier, Is.EqualTo(transactionId));
        });

        // Verify transaction was saved with correct values
        UnitOfWork.Verify(u => u.TransactionRepository.AddAsync(It.Is<CoreTransaction>(
            t => t.Amount == -75m &&
                 t.AbsoluteAmount == 75m &&
                 t.BeforeTransactionAmount == 200m &&
                 t.ResultingAmount == 125m
        )), Times.Once);
    }
}

// Made with Bob
