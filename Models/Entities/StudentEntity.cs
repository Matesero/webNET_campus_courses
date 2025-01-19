using courses.Models.enums;

namespace courses.Models.Entities;

public class StudentEntity
{
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }
    
    public Guid CourseId { get; set; }
    
    public CourseEntity Course { get; set; }
    
    public StudentStatuses Status { get; set; }
    
    public StudentMarks MidtermResult { get; set; }
    
    public StudentMarks FinalResult { get; set; }
}