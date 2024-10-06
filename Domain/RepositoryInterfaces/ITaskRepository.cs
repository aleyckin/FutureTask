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
        void Insert(Domain.Entities.Task task);
        void Remove(Domain.Entities.Task task);
    }
}
