using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class EditUserProfileModel
{
    [Required, MinLength(1)]
    public string fullName { get; set; }
    
    [Required]
    public DateTime birthDate { get; set; }
}