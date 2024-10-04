using Contracts.Dtos.SpecializationDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface ISpecializationService
    {
        Task<List<SpecializationDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SpecializationDto> GetSpecializationById(Guid specializationId, CancellationToken cancellationToken = default);
        Task<SpecializationDto> CreateAsync(SpecializationDtoForCreate specializationDtoForCreate, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid specializationId, SpecializationDtoForUpdate specializationDtoForUpdate, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid specializationId, CancellationToken cancellationToken = default);
    }
}
