using AutoMapper;
using Contracts.Dtos.AdministratorDtos;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public AdministratorService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<AdministratorDto> CreateAsync(AdministratorDtoForCreate administratorDtoForCreate, CancellationToken cancellationToken = default)
        {
            var administrator = _mapper.Map<Administrator>(administratorDtoForCreate);
            _repositoryManager.AdministratorRepository.Insert(administrator);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<AdministratorDto>(administrator);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid administratorId, CancellationToken cancellationToken = default)
        {
            var administrator = await _repositoryManager.AdministratorRepository.GetAdministratorByIdAsync(administratorId, cancellationToken);
            if (administrator == null)
            {
                throw new Exception();
            }
            _repositoryManager.AdministratorRepository.Remove(administrator);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<AdministratorDto> GetAdministratorById(Guid administratorId, CancellationToken cancellationToken = default)
        {
            var administrator = await _repositoryManager.AdministratorRepository.GetAdministratorByIdAsync(administratorId, cancellationToken);
            if (administrator == null)
            {
                throw new Exception();
            }
            var administratorDto = _mapper.Map<AdministratorDto>(administrator);
            return administratorDto;
        }

        public async Task<List<AdministratorDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var administrators = await _repositoryManager.AdministratorRepository.GetAllAdministratorsAsync(cancellationToken);
            var administratorsDto = _mapper.Map<List<AdministratorDto>>(administrators);
            return administratorsDto;
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid administratorId, AdministratorDtoForUpdate administratorDtoForUpdate, CancellationToken cancellationToken = default)
        {
            var administrator = await _repositoryManager.AdministratorRepository.GetAdministratorByIdAsync(administratorId, cancellationToken);
            if (administrator == null) 
            {
                throw new Exception();
            }
            if (administratorDtoForUpdate.Email != null) 
            {
                administrator.Email = administratorDtoForUpdate.Email;
            }
            if (administratorDtoForUpdate.Password != null)
            {
                administrator.Email = administratorDtoForUpdate.Password;
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
