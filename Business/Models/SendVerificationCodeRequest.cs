using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class SendVerificationCodeRequest
{
    [Required]
    public string Email { get; set; } = null!;
}
