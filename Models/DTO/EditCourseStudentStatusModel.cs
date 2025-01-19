using System.ComponentModel.DataAnnotations;
using courses.Models.enums;

namespace courses.Models.DTO;

public class EditCourseStudentStatusModel
{
    [Required]
    public StudentStatuses status { get; set; }
}