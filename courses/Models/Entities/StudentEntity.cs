using System.Text.Json.Serialization;
using courses.Models.enums;

namespace courses.Models.Entities;

public class StudentEntity
{
    private StudentEntity(
        Guid userId, 
        Guid courseId, 
        string status, 
        string midtermResult,
        string finalResult)
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
    
    [JsonIgnore]
    public CourseEntity Course { get; set; }
    
    public string Status { get; set; }
    
    public string MidtermResult { get; set; }
    
    public string FinalResult { get; set; }
    
    public static StudentEntity Create(
        Guid userId, 
        Guid courseId)
    {
        return new StudentEntity(
            userId, courseId,
            Enum.GetName(typeof(StudentStatuses), StudentStatuses.InQueue),
            Enum.GetName(typeof(StudentMarks), StudentMarks.NotDefined),
            Enum.GetName(typeof(StudentMarks), StudentMarks.NotDefined));
    }
}