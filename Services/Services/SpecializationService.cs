using AutoMapper;
using Contracts.Dtos.SpecializationDtos;
using Domain.Entities;
using Domain.Exceptions.SpecializationExceptions;
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
            var specialization = _mapper.Map<Specialization>(specializationDtoForCreate);
            _repositoryManager.SpecializationRepository.Insert(specialization);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<SpecializationDto>(specialization);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid specializationId, CancellationToken cancellationToken = default)
        {
            var specialization = await _repositoryManager.SpecializationRepository.GetSpecializationByIdAsync(specializationId, cancellationToken);
            if (specialization == null)
            {
                throw new SpecializationNotFoundException(specializationId);
            }
            _repositoryManager.SpecializationRepository.Remove(specialization);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<SpecializationDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var specializations = await _repositoryManager.SpecializationRepository.GetAllSpecializationsAsync(cancellationToken);
            return _mapper.Map<List<SpecializationDto>>(specializations);
        }

        public async Task<SpecializationDto> GetSpecializationById(Guid specializationId, CancellationToken cancellationToken = default)
        {
            var specialization = await _repositoryManager.SpecializationRepository.GetSpecializationByIdAsync(specializationId, cancellationToken);
            return _mapper.Map<SpecializationDto>(specialization);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid specializationId, SpecializationDtoForUpdate specializationDtoForUpdate, CancellationToken cancellationToken = default)
        {
            var specialization = await _repositoryManager.SpecializationRepository.GetSpecializationByIdAsync(specializationId, cancellationToken);
            if (specialization == null)
            {
                throw new SpecializationNotFoundException(specializationId);
            }
            if (specializationDtoForUpdate.Name != null)
            {
                specialization.Name = specializationDtoForUpdate.Name;
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
