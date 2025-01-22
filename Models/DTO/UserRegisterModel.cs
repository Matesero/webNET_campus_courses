using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class UserRegisterModel
{
    [Required, MinLength(1)]
    public string fullName { get; set; }
    
    [Required]
    public DateTime birthDate { get; set; }
    
    [Required, EmailAddress, MinLength(1)]
    public string email { get; set; }
    
    [Required, MinLength(6), MaxLength(32)]
    public string password { get; set; }
    
    [Required, MinLength(6), MaxLength(32)]
    public string confirmPassword { get; set; }
}