using courses.Models.enums;

namespace courses.Models.DTO;

public class CampusCourseFilterModel
{
    public SortList? sort { get; set; }

    public string? search { get; set; }
    
    public bool? hasPlacesAndOpen { get; set; }
    
    public Semesters? semester { get; set; }
    
    public int page { get; set; }

    public int pageSize { get; set; }
}