using Domain.Entities;
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

        public async Task<List<Domain.Entities.Task>> GetAllTasksForUserInColumnAsync(Guid userId, Guid columnId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Where(x => x.UserId == userId && x.ColumnId == columnId)
                .ToListAsync();
        }

        public async Task<List<Domain.Entities.Task>> GetAllTasksInColumnAsync(Guid columnId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Where(x => x.ColumnId == columnId)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Task> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Include(x => x.Conversation)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Guid> GetBestUserEmailForTask(Guid projectId)
        {
            List<User> usersInProject = await _dbContext.Users
                .Include(u => u.Tasks)
                .Where(u => u.ProjectUsers.Any(pu => pu.ProjectId == projectId))
                .ToListAsync();

            if (usersInProject.Count == 0)
            {
                throw new Exception("Нет пользователей в данном проекте.");
            }

            Guid[] projectColumnIdsArray = await _dbContext.Columns
               .Where(col => col.ProjectId == projectId)
               .Select(col => col.Id)
               .ToArrayAsync();

            HashSet<Guid> projectColumnIds = new HashSet<Guid>(projectColumnIdsArray);

            User bestUser = usersInProject
               .OrderBy(user =>
                   user.Tasks.Count(t => projectColumnIds.Contains(t.ColumnId)))
               .FirstOrDefault();

            if (bestUser != null && bestUser.Email != "")
            {
                return bestUser.Id;
            }
            else
            {
                throw new Exception("Пользователь с наименьшим числом задач не найден или отсутствует email.");
            }
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
