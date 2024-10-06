using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.TaskDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<TaskDto>> GetAllTasksForUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<TaskDto> GetTaskById(Guid taskId, CancellationToken cancellationToken = default);
        Task<TaskDto> CreateAsync(TaskDtoForCreate taskDtoForCreate, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid taskId, TaskDtoForUpdate taskDtoForUpdate, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid taskId, CancellationToken cancellationToken = default);
    }
}
