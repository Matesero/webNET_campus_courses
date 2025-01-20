using courses.Infrastructure;
using courses.Models.DTO;
using courses.Models.Entities;
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

    public GroupsService(IGroupsRepository groupsRepository)
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
        var group = _groupsRepository.GetById(id);

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
        var group = _groupsRepository.GetById(id);

        if (group is null)
        {
            throw new Exception(); // обработать
        }

        await _groupsRepository.Delete(id);
    }

    public async Task<List<CampusCoursePreviewModel>> GetCourses(Guid id)
    {
        var group = _groupsRepository.GetById(id);
        
        if (group is null)
        {
            throw new Exception(); // обработать
        }
        
        var courses = await _groupsRepository.GetCourses(id);

        return courses.Select(c => new CampusCoursePreviewModel
        {
            id = c.Id,
            name = c.Name,
            maximumStudentsCount = c.MaximumStudentsCount,
            remainingSlotsCount = c.RemainingSlotsCount,
            semester = c.Semester,
            startYear = c.StartYear,
            status = c.Status
        }).ToList();
    }
}