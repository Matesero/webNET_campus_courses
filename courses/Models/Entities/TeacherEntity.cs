using System.Text.Json.Serialization;

namespace courses.Models.Entities;

public class TeacherEntity
{
    private TeacherEntity(Guid userId, Guid courseId, bool isMain, Guid? groupId)
    {
        UserId = userId;
        CourseId = courseId;
        IsMain = isMain;
        GroupId = groupId;
    }
    
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }
    
    public Guid CourseId { get; set; }
    
    [JsonIgnore]
    public CourseEntity Course { get; set; }
    
    public Guid? GroupId { get; set; }
    
    public GroupEntity? Group { get; set; }
    
    public bool IsMain { get; set; }
    
    public static TeacherEntity Create(Guid userId, Guid courseId, bool isMain, Guid? groupId = null)
    {
        return new TeacherEntity(userId, courseId, isMain, groupId);
    }
}