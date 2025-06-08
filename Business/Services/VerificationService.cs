using Azure.Communication.Email;
using Microsoft.Extensions.Caching.Memory;
using Business.Models;
using Microsoft.Extensions.Configuration;
using Azure;
using System.Diagnostics;
using Business.Interfaces;

namespace Business.Services;

public class VerificationService(IConfiguration configuration, EmailClient emailClient, IMemoryCache cache) : IVerificationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly EmailClient _emailClient = emailClient;
    private readonly IMemoryCache _cache = cache;
    private static readonly Random _random = new();

    public async Task<VerificationServiceResult> SendVerificationCodeAsync(SendVerificationCodeRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return new VerificationServiceResult { Success = false, Error = "Email is requried" };
            }
            var verfficationCode = _random.Next(10000, 99999).ToString();
            var subject = $"Your code is {verfficationCode}";
            var plainTextContent = $@"
            Verify Your Email

            Hello,
            
            To complete your verification, please enter the following code:

            {verfficationCode}

            

            If you did not initiate this  request, don't worry your safety is not comprimised.

            @Ventixe. All right reserved.
        ";

            var htmlContent = $@"
            <!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <title>Verifieringskod</title>
  </head>
  <body>
    <div
      style=""
        display: flex;
        justify-content: center;
        align-items: center;
      ""
    >
      <div
        style=""
          background: white;
          padding: 2rem;
          border-radius: 1rem;
          box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
          max-width: 400px;
          width: 100%;
          text-align: center;
        ""
      >
        <form style=""width: 100%"">
          <label style=""display: block; font-size: 14px; margin-bottom: 2rem"">
            Verifieringskod
          </label>
          <p style=""font-size: 1.5rem; font-weight: bold; margin: 0"">
            {verfficationCode}
          </p>
        </form>
      </div>
    </div>
  </body>
</html>


        ";


            var emailMessage = new EmailMessage(
                senderAddress: _configuration["ACS:SenderAddress"],
                recipients: new EmailRecipients([new(request.Email)]),
                content: new EmailContent(subject)
                {
                    PlainText = plainTextContent,
                    Html = htmlContent
                });

            var emailSendOperation = await _emailClient.SendAsync(WaitUntil.Started, emailMessage);
            SaveVerificationCode(new SaveVerificationRequest { Email = request.Email, Code = verfficationCode, ValidFor = TimeSpan.FromMinutes(5) });

            return new VerificationServiceResult { Success = true, Message = "Email sent succeeded" };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return new VerificationServiceResult { Success = false, Error = "Email sent unsucceeded" };
        }
    }

    public void SaveVerificationCode(SaveVerificationRequest request)
    {
        _cache.Set(request.Email.ToLowerInvariant(), request.Code, request.ValidFor);
    }

    public VerificationServiceResult VerifyVerificationCode(VerifyVerificationCodeRequest request)
    {
        var key = request.Email.ToLowerInvariant();

        if (_cache.TryGetValue(key, out string? storedCode))
        {
            var isValid = storedCode == request.Code;

            if (isValid)
            {
                _cache.Remove(key);
                return new VerificationServiceResult
                {
                    Success = true, Message = "Verification code method successeded"
                };
            }
        }
        return new VerificationServiceResult
        {
            Success = false,
            Error = "Invalid or expired verification code"
        };
    }
}
