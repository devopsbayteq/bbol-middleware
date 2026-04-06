namespace BankCore.Integration.Security;

internal interface ITokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
