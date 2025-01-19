using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class EditCampusGroupModel
{
    [Required, MinLength(1)]
    public string name { get; set; }
}