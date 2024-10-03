using AutoMapper;
using Contracts.Dtos.SpecializationDtos;
using Domain.RepositoryInterfaces;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    class SpecializationService : ISpecializationService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public SpecializationService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<SpecializationDto> CreateAsync(SpecializationDtoForCreate specializationDtoForCreate, CancellationToken cancellationToken = default)
        {
            //var specialization = await _repositoryManager.SpecializationRepository
        }

        public Task DeleteAsync(Guid specializationId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<SpecializationDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SpecializationDto> GetSpecializationById(Guid specializationId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid specializationId, SpecializationDtoForUpdate specializationDtoForUpdate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
