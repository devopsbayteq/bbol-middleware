using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler para encriptar texto con AES.
/// </summary>
public class AesEncryptTextHandler(
    ILogger<AesEncryptTextHandler> logger,
    IOptions<AppSetting> options
    ) : SecurityBase<AesEncryptTextRequest, AesEncryptTextResponse>(logger)
{
    public override async Task<AesEncryptTextResponse> Handle(AesEncryptTextRequest request, CancellationToken cancellationToken)
        => await Task.FromResult(new AesEncryptTextResponse
        {
            EncryptText = AesSecurity.EncryptAes(request.PlainText, options.Value.AesSecurity.Key)
        }).ConfigureAwait(false);
}
