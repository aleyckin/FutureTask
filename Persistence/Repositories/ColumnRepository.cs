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
    public class ColumnRepository : IColumnRepository
    {
        private readonly RepositoryDbContext _dbContext;

        public ColumnRepository(RepositoryDbContext dbContext) { _dbContext = dbContext; }

        public async Task<List<Column>> GetAllColumnsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Columns
                .Include(x => x.Tasks)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Column>> GetAllColumnsForProjectAsync(Guid ProjectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Columns
                .Where(x => x.ProjectId == ProjectId)
                .Include(x => x.Tasks)
                .ToListAsync(cancellationToken);
        }

        public async Task<Column> GetColumnByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Columns
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Insert(Column column)
        {
            _dbContext.Add(column);
        }

        public void Remove(Column column)
        {
            _dbContext.Remove(column);
        }
    }
}
