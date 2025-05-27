using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class SaveVerificationRequest
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Code { get; set; } = null!;
    public TimeSpan ValidFor { get; set; }
}
