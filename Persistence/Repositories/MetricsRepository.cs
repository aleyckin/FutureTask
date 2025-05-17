using Domain.Entities;
using Domain.RepositoryInterfaces;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class MetricsRepository : IMetricsRepository
    {
        private readonly RepositoryDbContext _dbContext;

        public MetricsRepository(RepositoryDbContext dbContext) { _dbContext = dbContext; }

        public async System.Threading.Tasks.Task AddAsync(TaskMetrics metrics)
        {
            _dbContext.TaskMetrics.Add(metrics);
            await _dbContext.SaveChangesAsync();
        }
    }
}
