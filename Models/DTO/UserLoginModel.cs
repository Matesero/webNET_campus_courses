using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class UserLoginModel
{
    [Required, EmailAddress, MinLength(1)]
    public string email { get; set; }
    
    [Required, MinLength(1)]
    public string password { get; set; }
}