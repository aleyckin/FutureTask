using Contracts.Dtos.AdministratorDtos;
using Domain.RepositoryInterfaces;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IRepositoryManager _repositoryManager;

        public AdministratorService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }   

        public async Task<AdministratorDto> CreateAsync(AdministratorDtoForCreate administratorDtoForCreate, CancellationToken cancellationToken = default)
        {
            throw new Exception();
        }

        public Task DeleteAsync(Guid administratorId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AdministratorDto> GetAdministratorById(Guid administratorId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AdministratorDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var administrators = await _repositoryManager.AdministratorRepository.GetAllAdministratorsAsync(cancellationToken);
            var administratorsDto = administrators.Adapt<List<AdministratorDto>>();
            return administratorsDto;
        }

        public Task UpdateAsync(Guid administratorId, AdministratorDtoForUpdate administratorDtoForUpdate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
