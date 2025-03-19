using System.Runtime.Serialization;

namespace courses.Models.enums;

public enum MarkType
{
    [EnumMember(Value = "Midterm")]
    Midterm,
    
    [EnumMember(Value = "Final")]
    Final
}