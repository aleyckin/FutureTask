using AutoMapper;
using Contracts.Dtos.UserDtos;
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
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public UserService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<UserDto> CreateAsync(UserDtoForCreate userDtoForCreate, CancellationToken cancellationToken = default)
        {
            var user = _mapper.Map<User>(userDtoForCreate);
            _repositoryManager.UserRepository.Insert(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserDto>(user);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new Exception();
            }
            _repositoryManager.UserRepository.Remove(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _repositoryManager.UserRepository.GetAllUsersAsync(cancellationToken);
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetUserById(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid userId, UserDtoForUpdate userDtoForUpdate, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new Exception();
            }
            if (userDtoForUpdate.Email != null)
            {
                user.Email = userDtoForUpdate.Email;
            }
            if (userDtoForUpdate.Password != null)
            {
                user.Password = userDtoForUpdate.Password;
            }
            if (userDtoForUpdate.SpecializationId != Guid.Empty)
            {
                user.SpecializationId = userDtoForUpdate.SpecializationId;
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
