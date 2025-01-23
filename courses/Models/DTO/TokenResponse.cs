using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class TokenResponse
{
    [Required, MinLength(1)]
    public string token { get; set; }
}