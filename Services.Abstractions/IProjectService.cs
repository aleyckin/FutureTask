using Contracts.Dtos.AdministratorDtos;
using Contracts.Dtos.ProjectDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProjectDto> GetProjectById(Guid projectId, CancellationToken cancellationToken = default);
        Task<ProjectDto> CreateAsync(ProjectDtoForCreate projectDtoForCreate, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid projectId, ProjectDtoForUpdate projectDtoForUpdate, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid projectId, CancellationToken cancellationToken = default);
    }
}
