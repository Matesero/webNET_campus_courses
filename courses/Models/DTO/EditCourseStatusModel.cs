using System.ComponentModel.DataAnnotations;
using courses.Models.enums;

namespace courses.Models.DTO;

public class EditCourseStatusModel
{
    [Required]
    public string status { get; set; }
}