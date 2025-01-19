using System.ComponentModel.DataAnnotations;
using courses.Models.enums;

namespace courses.Models.DTO;

public class EditCourseStudentMarkModel
{
    [Required]
    public MarkType markType { get; set; }
    
    [Required]
    public StudentMarks mark { get; set; }
}