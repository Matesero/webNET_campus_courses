using System.ComponentModel.DataAnnotations;
using courses.Models.enums;

namespace courses.Models.DTO;

public class CreateCampusCourseModel
{
    [Required, MinLength(1)]
    public string name { get; set; }
    
    [Required, Range(2000, 2029)]
    public int startYear { get; set; }
    
    [Required, Range(1, 200)]
    public int maximumStidentsCount { get; set; }
    
    [Required]
    public Semesters semester { get; set; }
    
    [Required]
    public string requirements { get; set; }
    
    [Required]
    public string annotations { get; set; }
    
    [Required]
    public Guid mainTeacherId { get; set; }
}