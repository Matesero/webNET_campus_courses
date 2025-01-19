using courses.Models.enums;

namespace courses.Models.DTO;

public class CampusCourseStudentModel
{
    public Guid id { get; set; }
    
    public string? name { get; set; }
    
    public string? email { get; set; }
    
    public StudentStatuses status { get; set; }
    
    public StudentMarks midtermStatus  { get; set; }
    
    public StudentMarks? finalResult { get; set; }
}