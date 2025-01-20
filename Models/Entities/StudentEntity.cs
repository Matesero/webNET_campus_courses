using courses.Models.enums;

namespace courses.Models.Entities;

public class StudentEntity
{
    private StudentEntity(
        Guid userId, 
        Guid courseId, 
        StudentStatuses status, 
        StudentMarks midtermResult,
        StudentMarks finalResult)
    {
        UserId = userId;
        CourseId = courseId;
        Status = status;
        MidtermResult = midtermResult;
        FinalResult = finalResult;
    }
        
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }
    
    public Guid CourseId { get; set; }
    
    public CourseEntity Course { get; set; }
    
    public StudentStatuses Status { get; set; }
    
    public StudentMarks MidtermResult { get; set; }
    
    public StudentMarks FinalResult { get; set; }
    
    public static StudentEntity Create(
        Guid userId, 
        Guid courseId)
    {
        return new StudentEntity(
            userId, courseId, 
            StudentStatuses.InQueue, 
            StudentMarks.NotDefined, 
            StudentMarks.NotDefined);
    }
}