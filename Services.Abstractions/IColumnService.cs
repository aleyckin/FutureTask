using Contracts.Dtos.ColumnDtos;
using Contracts.Dtos.ProjectDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IColumnService
    {
        Task<List<ColumnDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<ColumnDto>> GetAllColumnsForProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task<ColumnDto> GetColumnById(Guid columnId, CancellationToken cancellationToken = default);
        Task<ColumnDto> CreateAsync(ColumnDtoForCreate columnDtoForCreate, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid columnId, ColumnDtoForUpdate columnDtoForUpdate, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid columnId, CancellationToken cancellationToken = default);
    }
}
