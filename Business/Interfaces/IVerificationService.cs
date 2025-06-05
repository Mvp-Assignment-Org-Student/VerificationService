using Business.Models;

namespace Business.Interfaces;

public interface IVerificationService
{
    Task<VerificationServiceResult> SendVerificationCodeAsync(SendVerificationCodeRequest request);

    void SaveVerificationCode(SaveVerificationRequest request);

    VerificationServiceResult VerifyVerificationCode(VerifyVerificationCodeRequest request);
}