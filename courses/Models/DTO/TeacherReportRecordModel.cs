﻿namespace courses.Models.DTO;

public class TeacherReportRecordModel
{
    public string? fullName { get; set; }
    
    public Guid id { get; set; }
    
    public List<CampusGroupReportModel>? campusGroupReports { get; set; }
}