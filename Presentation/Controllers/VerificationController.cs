using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VerificationController(IVerificationService verificationService) : ControllerBase
{
    private readonly IVerificationService _verificationService = verificationService;


    [HttpPost("send")]
    public async Task<IActionResult> Send(SendVerificationCodeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Error = "Recipient email addres is requried" });
        }

        var result = await _verificationService.SendVerificationCodeAsync(request);
        return result.Success
            ? Ok(result)
            : StatusCode(500, result);
    }

    [HttpPost("verify")]
    public IActionResult Verify(VerifyVerificationCodeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Error = "Invalid or  expired  verification code. 'Controller'" });
        }
        var result = _verificationService.VerifyVerificationCode(request);

        return result.Success
           ? Ok(result)
           : StatusCode(500, result);
    }
}
