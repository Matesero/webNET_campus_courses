using courses.Infrastructure;
using courses.Models.DTO;
using courses.Models.Entities;
using courses.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace courses.Services;

public interface IGroupsService
{
    
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
}