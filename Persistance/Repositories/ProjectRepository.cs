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
            throw new NotImplementedException();
        }

        public Task<Project> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Insert(Project project)
        {
            throw new NotImplementedException();
        }

        public void Remove(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
