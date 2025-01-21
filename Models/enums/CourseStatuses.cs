using System.Runtime.Serialization;

namespace courses.Models.enums;

public enum CourseStatuses
{
    [EnumMember(Value = "Created")]
    Created = 1,
    
    [EnumMember(Value = "OpenForAssigning")]
    OpenForAssigning = 2,
    
    [EnumMember(Value = "Started")]
    Started = 3,
    
    [EnumMember(Value = "Finished")]
    Finished = 4
}