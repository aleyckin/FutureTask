using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.ProjectUsersDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IProjectUsersService
    {
        Task<List<ProjectUsersDtoForListUsers>> GetAllUsersByProject(Guid ProjectId, CancellationToken cancellationToken = default);
        Task<List<ProjectUsersDtoForListProjects>> GetAllProjectsByUser(Guid UserId, CancellationToken cancellationToken = default);
        Task AddUserToProjectAsync(ProjectUsersDto projectUsersDto, CancellationToken cancellationToken = default);
        Task UpdateUserRoleInProjectAsync(ProjectUsersDto projectUsersDto, CancellationToken cancellationToken = default);
        Task DeleteUserFromProjectAsync(ProjectUsersDto projectUsersDto, CancellationToken cancellationToken = default);
        Task<RoleOnProject?> GetUserRoleOnProject(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
    }
}
