using courses.Models.enums;

namespace courses.Models.DTO;

public class CampusCourseStudentModel
{
    public Guid id { get; set; }
    
    public string? name { get; set; }
    
    public string? email { get; set; }
    
    public string status { get; set; }
    
    public string midtermStatus  { get; set; }
    
    public string? finalResult { get; set; }
}