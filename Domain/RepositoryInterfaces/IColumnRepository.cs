using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IColumnRepository
    {
        Task<List<Column>> GetAllColumnsAsync(CancellationToken cancellationToken = default);
        Task<List<Column>> GetAllColumnsForProjectAsync(Guid ProjectId, CancellationToken cancellationToken = default);
        Task<Column> GetColumnByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Insert(Column column);
        void Remove(Column column);
    }
}
