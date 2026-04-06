using LogicApi.Model.Enums;
using LogicApi.Model.Request.Beneficiary;
using LogicApi.Model.Response.Beneficiary;
using Microsoft.Extensions.Logging;
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
        var beneficiaries = await unitOfWork.BeneficiaryRepository.GetByAsync().ConfigureAwait(false);
        var response = new GetBeneficiaryContactsResponse
        {
            Contacts = [.. beneficiaries.Select(b =>
            {
                var source = string.IsNullOrWhiteSpace(b.AccountNumber) ? b.Identification : b.AccountNumber;
                var lastFour = source.Length >= 4 ? source[^4..] : source;
                return new BeneficiaryContactItem
                {
                    BeneficiaryGuid = b.Id,
                    ContactName = b.Name,
                    BankName = b.BankName,
                    AccountType = (AccountType)b.AccountType,
                    BeneficiaryAccountNumber = b.AccountNumber ?? string.Empty,
                    LastFourDigits = lastFour
                };
            })]
        };

        return response;
    }
}
