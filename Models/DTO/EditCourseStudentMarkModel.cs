using System.ComponentModel.DataAnnotations;
using courses.Models.enums;

namespace courses.Models.DTO;

public class EditCourseStudentMarkModel
{
    [Required]
    public string markType { get; set; }
    
    [Required]
    public string mark { get; set; }
}