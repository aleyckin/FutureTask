using Contracts.Dtos.AdministratorDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IAdministratorService
    {
        Task<List<AdministratorDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<AdministratorDto> GetAdministratorById(Guid administratorId, CancellationToken cancellationToken = default);
        Task<AdministratorDto> CreateAsync(AdministratorDtoForCreate administratorDtoForCreate, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid administratorId, AdministratorDtoForUpdate administratorDtoForUpdate, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid administratorId, CancellationToken cancellationToken = default);
    }
}
