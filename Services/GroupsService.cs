using courses.Infrastructure;
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

        if (group is null)
        {
            throw new Exception(); // обработать
        }

        await _groupsRepository.Update(id, name);

        return new CampusGroupModel
        {
            id = id,
            name = name
        };
    }
    
    public async Task Delete(Guid id)
    {
        var group = await _groupsRepository.GetById(id);

        if (group is null)
        {
            throw new Exception(); // обработать
        }

        await _groupsRepository.Delete(id);
    }

    public async Task<List<CampusCoursePreviewModel>> GetCourses(Guid id)
    {
        var group = await _groupsRepository.GetById(id);
        
        if (group is null)
        {
            throw new Exception(); // обработать
        }
        
        var courses = await _groupsRepository.GetCourses(id);

        return courses.Select(course =>
        {
            return new CampusCoursePreviewModel
            {
                id = course.Id,
                name = course.Name,
                startYear = course.StartYear,
                maximumStudentsCount = course.MaximumStudentsCount,
                remainingSlotsCount = Math.Max(0, course.MaximumStudentsCount - course.Students.Count),
                semester = course.Semester,
                status = course.Status
            };
        }).ToList();
    }

    public async Task<List<TeacherReportRecordModel>> GetReports(Semesters? semester, List<Guid> groupIds)
    {
        var mainTeachers = await _groupsRepository.GetWithDetailedCourses(semester.ToString(), groupIds);

        var report = mainTeachers
            .Select(t =>
            {
                return new TeacherReportRecordModel
                {
                    id = t.UserId,
                    fullName = t.User.FullName,
                    campusGroupReports = new List<CampusGroupReportModel>()
                };
            })
            .GroupBy(r => r.id)  
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
                        throw new Exception();
                    }
                    
                    var averagePassed = mainTeachers
                        .Where(t => t.GroupId == groupId && t.UserId == mt.id)  
                        .Select(t => t.Group) 
                        .SelectMany(g => g.Courses)  
                        .Select(c => new 
                        {
                            Course = c,
                            PassedCount = c.Students.Count(st => st.FinalResult == "Passed")  
                        })
                        .Average(course => course.PassedCount);
                    
                    var averageFailed = mainTeachers
                        .Where(t => t.GroupId == groupId && t.UserId == mt.id)  
                        .Select(t => t.Group) 
                        .SelectMany(g => g.Courses)  
                        .Select(c => new 
                        {
                            Course = c,
                            FailedCount = c.Students.Count(st => st.FinalResult == "Failed")  
                        })
                        .Average(course => course.FailedCount);

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