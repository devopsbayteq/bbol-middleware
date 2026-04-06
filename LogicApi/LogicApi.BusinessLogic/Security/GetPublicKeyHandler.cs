using System.Text;
using Common.WebApi.Models.AppSettings;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler for get public key operations
/// </summary>
public class GetPublicKeyHandler(
    ILogger<GetPublicKeyHandler> logger,
    IOptions<AppSetting> options
    ) : SecurityBase<GetPublicKeyRequest, GetPublicKeyResponse>(logger)
{
    /// <summary>
    /// Handle the get public key request
    /// </summary>
    /// <param name="request">Get public key request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Get public key response</returns>
    public override async Task<GetPublicKeyResponse> Handle(GetPublicKeyRequest request, CancellationToken cancellationToken)
        => await Task.FromResult(new GetPublicKeyResponse(Encoding.UTF8.GetString(Convert.FromBase64String(options.Value.RsaSecurity.ServerBase64PublicKey)))).ConfigureAwait(false);
}
