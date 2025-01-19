using System.ComponentModel.DataAnnotations;
using courses.Models.enums;

namespace courses.Models.DTO;

public class EditCourseStatusModel
{
    [Required]
    public CourseStatuses status { get; set; }
}