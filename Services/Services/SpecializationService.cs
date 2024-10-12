using AutoMapper;
using Contracts.Dtos.SpecializationDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using Domain.Entities.Enums;
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
    public class SpecializationService : ISpecializationService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public SpecializationService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async Task<SpecializationDto> CreateAsync(SpecializationDtoForCreate specializationDtoForCreate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(specializationDtoForCreate, cancellationToken);

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
            if (specialization == null)
            {
                throw new SpecializationNotFoundException(specializationId);
            }
            return _mapper.Map<SpecializationDto>(specialization);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid specializationId, SpecializationDtoForUpdate specializationDtoForUpdate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(specializationDtoForUpdate, cancellationToken);

            var specialization = await _repositoryManager.SpecializationRepository.GetSpecializationByIdAsync(specializationId, cancellationToken);
            if (specialization == null)
            {
                throw new SpecializationNotFoundException(specializationId);
            }

            _mapper.Map(specializationDtoForUpdate, specialization);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task SeedSpecializationUserAsync(CancellationToken cancellationToken = default)
        {
            var adminSpecialization = await _repositoryManager.SpecializationRepository.GetSpecializationByNameAsync("adminSpecialization", cancellationToken);

            if (adminSpecialization == null)
            {
                var adminSpecializationDto = new SpecializationDtoForCreate("adminSpecialization");
                await CreateAsync(adminSpecializationDto, cancellationToken);
            }
        }
    }
}
