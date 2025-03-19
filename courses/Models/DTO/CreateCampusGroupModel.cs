using System.ComponentModel.DataAnnotations;

namespace courses.Models.DTO;

public class CreateCampusGroupModel
{
    [Required, MinLength(1)]
    public string name { get; set; }
}