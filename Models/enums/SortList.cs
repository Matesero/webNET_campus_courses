using System.Runtime.Serialization;

namespace courses.Models.enums;

public enum SortList
{
    [EnumMember(Value = "CreatedAsc")]
    CreatedAsc,
    
    [EnumMember(Value = "CreatedDesc")]
    CreatedDesc,
}