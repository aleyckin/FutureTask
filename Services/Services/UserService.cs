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

            byte[] salt;
            user.Password = PasswordHasher.HashPassword(userDtoForCreate.Password, out salt);
            user.PasswordSalt = salt;

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

        public Task<UserDto> GetUserByEmail(string email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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

                byte[] salt;
                user.Password = PasswordHasher.HashPassword(userDtoForUpdate.Password, out salt);
                user.PasswordSalt = salt; 
            }
            if (userDtoForUpdate.SpecializationId != Guid.Empty)
            {
                user.SpecializationId = userDtoForUpdate.SpecializationId;
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ValidateUserCredentials(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetUserByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                throw new Exception();
            }
            bool IsValidPassword = PasswordHasher.VerifyPassword(password, user.Password, user.PasswordSalt);
            return IsValidPassword;
        }
    }
}
