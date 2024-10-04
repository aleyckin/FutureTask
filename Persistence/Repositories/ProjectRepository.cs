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
    public class ProjectRepository : IProjectRepository
    {
        private readonly RepositoryDbContext _dbContext;
        public ProjectRepository(RepositoryDbContext dbContext) { _dbContext = dbContext; }

        public async Task<List<Project>> GetAllProjectsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Projects
                .Include(x => x.Columns)
                .Include(x => x.ProjectUsers)
                .ThenInclude(x => x.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<Project> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Projects
                .Include(x => x.Columns)
                .Include(x => x.ProjectUsers)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Insert(Project project)
        {
            _dbContext.Add(project);
        }

        public void Remove(Project project)
        {
            _dbContext.Remove(project);
        }
    }
}
