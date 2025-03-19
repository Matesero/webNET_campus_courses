using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class EditCampusCourseRequirementsAndAnnotationsModel
{
    [Required, MinLength(1)]
    public string requirements { get; set; }
    
    [Required, MinLength(1)]
    public string annotations { get; set; }
}