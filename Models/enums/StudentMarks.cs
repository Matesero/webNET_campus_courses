using System.Runtime.Serialization;

namespace courses.Models.enums;

public enum StudentMarks
{
    [EnumMember(Value = "NotDefined")]
    NotDefined,
    
    [EnumMember(Value = "Passed")]
    Passed,
    
    [EnumMember(Value = "Failed")]
    Failed
}