using System.Collections.Concurrent;
using Common.WebApi.Exceptions;
using Common.WebApi.Extensions;
using Common.WebApi.Messages;
using Common.WebApi.Models.AppSettings;
using Common.WebApi.Security;
using LogicApi.Model.Request.Security;
using LogicApi.Model.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicApi.BusinessLogic.Security;

/// <summary>
/// Handler para validacion de OTP
/// </summary>
public class ValidateOtpHandler(
    ILogger<ValidateOtpHandler> logger,
    IOptions<AppSetting> options
    ) : SecurityBase<ValidateOtpRequest, GenericCommonOperationResponse>(logger)
{

    /// <summary>
    /// Handle the validate OTP request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<GenericCommonOperationResponse> Handle(ValidateOtpRequest request, CancellationToken cancellationToken)
    {
        var decodeOtp = request.Otp.Decode();
        var decryptOtp = RsaSecurity.Decrypt(options.Value.RsaSecurity.ServerBase64PrivateKey, decodeOtp);
        if (!decryptOtp.Equals(options.Value.Otp.SecretKey))
            throw new CustomException(MessageCodes.OtpIncorrect, "El código ingresado es incorrecto.");
        return Task.FromResult(GenericCommonOperationResponse.SuccessOperation());
    }
}
