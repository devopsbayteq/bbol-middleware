using LogicApi.Model.Enums;
using LogicApi.Model.Request.Beneficiary;
using LogicApi.Model.Response.Beneficiary;
using Microsoft.Extensions.Logging;
using PersistenceDb.Models.Enums;
using PersistenceDb.Repository.Interfaces.UnitOfWork;

namespace LogicApi.BusinessLogic.Beneficiary;

/// <summary>
/// Handler para lista de contactos beneficiarios
/// </summary>
public class GetBeneficiaryContactsHandler(
    ILogger<GetBeneficiaryContactsHandler> logger,
    IUnitOfWork unitOfWork
    ) : BeneficiaryBase<GetBeneficiaryContactsRequest, GetBeneficiaryContactsResponse>(logger)
{
    public override async Task<GetBeneficiaryContactsResponse> Handle(GetBeneficiaryContactsRequest request, CancellationToken cancellationToken)
    {
        var userId = request.ContextRequest?.CustomClaims?.UserId;
        var beneficiaryType = request.BeneficiaryType switch
        {
            BeneficiaryType.OwnAccounts => BeneficiaryTypeId.OwnAccounts,
            BeneficiaryType.ExternalAccounts => BeneficiaryTypeId.ExternalAccounts,
            _ => throw new InvalidOperationException("Tipo de beneficiario no válido."),
        };
        var beneficiaries = await unitOfWork.BeneficiaryRepository.GetByAsync(
            where => where.UserId == userId && where.BeneficiaryType == beneficiaryType
        ).ConfigureAwait(false);
        var response = new GetBeneficiaryContactsResponse
        {
            Contacts = [.. beneficiaries.Select(b =>
            {
                var source = string.IsNullOrWhiteSpace(b.AccountNumber) ? b.Identification : b.AccountNumber;
                string lastThree = source.Length >= 3 ? source[^3..] : source;
                string masked = string.Concat(Enumerable.Repeat("*", Math.Max(0, source.Length - 3))) + lastThree;
                return new BeneficiaryContactItem
                {
                    BeneficiaryGuid = b.Id,
                    ContactName = b.Name,
                    BankName = b.BankName,
                    AccountType = (Model.Enums.AccountType)b.AccountType,
                    BeneficiaryAccountNumber = b.AccountNumber ?? string.Empty,
                    LastFourDigits = masked
                };
            })]
        };

        return response;
    }
}
