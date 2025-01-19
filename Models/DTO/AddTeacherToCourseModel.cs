using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class AddTeacherToCourseModel
{
    [Required]
    public Guid userId { get; set; }
}