using System.Runtime.Serialization;

namespace courses.Models.enums;

public enum SortList
{
    [EnumMember(Value = "CreatedAsc")]
    CreatedAsc = 1,
    
    [EnumMember(Value = "CreatedDesc")]
    CreatedDesc = 2,
}