namespace BankCore.Integration.Security;

public interface ITokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
