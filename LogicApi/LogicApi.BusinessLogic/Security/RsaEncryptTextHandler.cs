using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using Common.WebApi.Extensions;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler para encriptar texto con RSA (llave pública del servidor).
/// </summary>
public class RsaEncryptTextHandler(
    ILogger<RsaEncryptTextHandler> logger,
    IOptions<AppSetting> options
    ) : SecurityBase<RsaEncryptTextRequest, RsaEncryptTextResponse>(logger)
{
    public override async Task<RsaEncryptTextResponse> Handle(RsaEncryptTextRequest request, CancellationToken cancellationToken)
        => await Task.FromResult(new RsaEncryptTextResponse
        {
            EncryptText = RsaSecurity.Encrypt(options.Value.RsaSecurity.ServerBase64PublicKey, request.PlainText).Encode()
        }).ConfigureAwait(false);
}
