using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using Common.WebApi.Extensions;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler para desencriptar texto con RSA (llave privada del servidor).
/// </summary>
public class RsaDecryptTextHandler(
    ILogger<RsaDecryptTextHandler> logger,
    IOptions<AppSetting> options
    ) : SecurityBase<RsaDecryptTextRequest, RsaDecryptTextResponse>(logger)
{
    public override async Task<RsaDecryptTextResponse> Handle(RsaDecryptTextRequest request, CancellationToken cancellationToken)
        => await Task.FromResult(new RsaDecryptTextResponse
        {
            PlainText = RsaSecurity.Decrypt(options.Value.RsaSecurity.ServerBase64PrivateKey, request.EncryptText.Decode())
        }).ConfigureAwait(false);
}
