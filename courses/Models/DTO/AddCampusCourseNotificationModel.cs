using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class AddCampusCourseNotificationModel
{
    [Required, MinLength(1)]
    public string text { get; set; }
    
    [Required]
    public bool isImportant { get; set; }
}