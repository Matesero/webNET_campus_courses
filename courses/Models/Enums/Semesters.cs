using System.Runtime.Serialization;

namespace courses.Models.enums;

public enum Semesters
{
    [EnumMember(Value = "Autumn")]
    Autumn,
    
    [EnumMember(Value = "Spring")]
    Spring
}