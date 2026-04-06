using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler para desencriptar texto con AES.
/// </summary>
public class AesDecryptTextHandler(
    ILogger<AesDecryptTextHandler> logger,
    IOptions<AppSetting> options
    ) : SecurityBase<AesDecryptTextRequest, AesDecryptTextResponse>(logger)
{
    public override async Task<AesDecryptTextResponse> Handle(AesDecryptTextRequest request, CancellationToken cancellationToken)
        => await Task.FromResult(new AesDecryptTextResponse
        {
            PlainText = AesSecurity.DecryptAes(request.EncryptText, options.Value.AesSecurity.Key)
        }).ConfigureAwait(false);
}
