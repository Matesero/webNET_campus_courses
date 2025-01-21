using System.Runtime.Serialization;

namespace courses.Models.enums;

public enum StudentStatuses
{
    [EnumMember(Value = "InQueue")]
    InQueue,
    
    [EnumMember(Value = "Accepted")]
    Accepted,
    
    [EnumMember(Value = "Declined")]
    Declined
}