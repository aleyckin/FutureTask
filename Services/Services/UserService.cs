﻿using AutoMapper;
using Contracts.Dtos.TaskDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Exceptions.SpecializationExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IValidatorManager _validatorManager;

        public UserService(IRepositoryManager repositoryManager, IMapper mapper, IConfiguration configuration, IValidatorManager validatorManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _configuration = configuration;
            _validatorManager = validatorManager;
        }

        public async Task<UserDto> CreateAsync(UserDtoForCreate userDtoForCreate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(userDtoForCreate, cancellationToken);

            var specialization = await _repositoryManager.SpecializationRepository.GetSpecializationByIdAsync(userDtoForCreate.SpecializationId, cancellationToken);
            if (specialization == null)
            {
                throw new SpecializationNotFoundException(userDtoForCreate.SpecializationId);
            }

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
                throw new UserNotFoundException(userId);
            }
            _repositoryManager.UserRepository.Remove(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _repositoryManager.UserRepository.GetAllUsersAsync(cancellationToken);
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByEmail(string email, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetUserByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundEmailException(email);
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserById(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            return _mapper.Map<UserDto>(user);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid userId, UserDtoForUpdate userDtoForUpdate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(userDtoForUpdate, cancellationToken);

            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            var specialization = await _repositoryManager.SpecializationRepository.GetSpecializationByIdAsync((Guid)userDtoForUpdate.SpecializationId, cancellationToken);
            if (specialization == null)
            {
                throw new SpecializationNotFoundException((Guid)userDtoForUpdate.SpecializationId);
            }

            _mapper.Map(userDtoForUpdate, user);

            if (userDtoForUpdate.Password != null)
            {

                byte[] salt;
                user.Password = PasswordHasher.HashPassword(userDtoForUpdate.Password, out salt);
                user.PasswordSalt = salt; 
            }

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserDto> ValidateUserCredentials(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetUserByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundEmailException(email);
            }
            bool IsValidPassword = PasswordHasher.VerifyPassword(password, user.Password, user.PasswordSalt);
            if (IsValidPassword)
            {
                return _mapper.Map<UserDto>(user);
            }
            throw new UserCredentialsException();
        }

        public string GenerateJwtToken(UserDto userDto)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", userDto.Id.ToString()),
                new Claim(ClaimTypes.Role, userDto.UserRole.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async System.Threading.Tasks.Task SeedAdminUserAsync(CancellationToken cancellationToken = default)
        {
            var adminUser = await _repositoryManager.UserRepository.GetUserByEmailAsync("admin@admin", cancellationToken);

            if (adminUser == null)
            {
                var specialization = await _repositoryManager.SpecializationRepository.GetSpecializationByNameAsync("adminSpecialization", cancellationToken);
                if (specialization == null)
                {
                    throw new Exception();
                }
                var adminUserDto = new UserDtoForCreate("admin@admin", "password", specialization.Id, UserRole.Administrator);
                await CreateAsync(adminUserDto, cancellationToken);
            }
        }
    }
}
