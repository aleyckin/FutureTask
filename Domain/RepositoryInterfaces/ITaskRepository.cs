using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ITaskRepository
    {
        Task<List<Domain.Entities.Task>> GetAllTasksAsync(CancellationToken cancellationToken = default);
        Task<List<Domain.Entities.Task>> GetAllTasksForUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Domain.Entities.Task> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Domain.Entities.Task>> GetAllTasksForUserInColumnAsync(Guid userId, Guid columnId, CancellationToken cancellationToken = default);
        Task<List<Domain.Entities.Task>> GetAllTasksInColumnAsync(Guid columnId, CancellationToken cancellationToken = default);
        Task<Guid> GetBestUserEmailForTask(Guid projectId);
        void Insert(Domain.Entities.Task task);
        void Remove(Domain.Entities.Task task);
    }
}
