using Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly RepositoryDbContext _dbContext;

        public TaskRepository(RepositoryDbContext dbContext) { _dbContext = dbContext; }

        public async Task<List<Domain.Entities.Task>> GetAllTasksAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Domain.Entities.Task>> GetAllTasksForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Task> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Insert(Domain.Entities.Task task)
        {
            _dbContext.Add(task);
        }

        public void Remove(Domain.Entities.Task task)
        {
            _dbContext.Remove(task);
        }
    }
}
