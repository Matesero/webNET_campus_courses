using courses.Infrastructure;
using courses.Middleware;
using courses.Models.DTO;
using courses.Models.Entities;
using courses.Models.enums;
using courses.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace courses.Services;

public interface IGroupsService
{
    Task<List<CampusGroupModel>> GetAll();

    Task<CampusGroupModel> Create(string name);

    Task<CampusGroupModel> Edit(Guid id, string name);

    Task Delete(Guid id);

    Task<List<CampusCoursePreviewModel>> GetCourses(Guid id);
}

public class GroupsService : IGroupsService
{
    private readonly IGroupsRepository _groupsRepository;

    public GroupsService(IGroupsRepository groupsRepository, ICoursesRepository coursesRepository)
    {
        _groupsRepository = groupsRepository;
    }
    
    public async Task<List<CampusGroupModel>> GetAll()
    {
        var groups = await _groupsRepository.GetAll();
        
        return groups;
    }

    public async Task<CampusGroupModel> Create(string name)
    {
        var id = Guid.NewGuid();
        
        var group = GroupEntity.Create(id, name);
        await _groupsRepository.Add(group);
        
        return new CampusGroupModel
        {
            id = id,
            name = name
        };
    }

    public async Task<CampusGroupModel> Edit(Guid id, string name)
    {
        var group = await _groupsRepository.GetById(id);

        group.Name = name;
        await _groupsRepository.Update(group);

        return new CampusGroupModel
        {
            id = id,
            name = name
        };
    }
    
    public async Task Delete(Guid id)
    {
        await _groupsRepository.Delete(id);
    }

    public async Task<List<CampusCoursePreviewModel>> GetCourses(Guid id)
    {
        await _groupsRepository.CheckExistence(id);
        
        var courses = await _groupsRepository.GetCourses(id);

        return courses.Select(course =>
        {
            return new CampusCoursePreviewModel
            {
                id = course.Id,
                name = course.Name,
                startYear = course.StartYear,
                maximumStudentsCount = course.MaximumStudentsCount,
                remainingSlotsCount = Math.Max(0, course.MaximumStudentsCount - course.Students.Count(student => 
                    student.Status == Enum.GetName(StudentStatuses.Accepted))),
                semester = course.Semester,
                status = course.Status
            };
        }).ToList();
    }

    public async Task<List<TeacherReportRecordModel>> GetReports(Semesters? semester, List<Guid> groupIds)
    {
        var mainTeachers = await _groupsRepository.GetWithDetailedCourses(semester.ToString(), groupIds);

        var report = mainTeachers
            .Where(t => t.Group.Courses.Count > 0)
            .Select(t =>
            {
                return new TeacherReportRecordModel
                {
                    id = t.UserId,
                    fullName = t.User.FullName,
                    campusGroupReports = new List<CampusGroupReportModel>()
                };
            })
            .GroupBy(t => t.id)  
            .Select(g => g.First())
            .ToList();
        
        report.ForEach(mt =>
        {
            {
                var uniqueGroups = mainTeachers
                    .Where(t => t.UserId == mt.id)
                    .Select(t => new { id = t.GroupId, name = t.Group.Name })
                    .Distinct() 
                    .ToList();

                uniqueGroups.ForEach(group =>
                {
                    if (!Guid.TryParse(group.id.ToString(), out var groupId))
                    {
                        throw new BadRequestException("Invalid Group ID");
                    }
                    
                    var courses = mainTeachers
                        .Where(t => t.GroupId == groupId && t.UserId == mt.id)
                        .SelectMany(t => t.Group.Courses
                            .Where(c => c.MainTeacherId == mt.id));
                    
                    var passedCounts = courses
                        .Select(c => c.Students.Count(st => st.FinalResult == "Passed"))
                        .ToList();

                    var averagePassed = passedCounts.Any() ? passedCounts.Average() : 0;
                    
                    var failedCounts = courses
                        .Select(c => c.Students.Count(st => st.FinalResult == "Failed"))
                        .ToList(); 

                    var averageFailed = failedCounts.Any() ? failedCounts.Average() : 0;

                    mt.campusGroupReports.Add(new CampusGroupReportModel
                    {
                        name = group.name,
                        id = groupId,
                        averagePassed = averagePassed,
                        averageFailed = averageFailed
                    });
                });
            }
        });
        
        return report;
    }
}