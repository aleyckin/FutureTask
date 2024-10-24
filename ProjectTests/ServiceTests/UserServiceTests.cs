using AutoMapper;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Exceptions.SpecializationExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Services;
using Services.Abstractions;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProjectTests.ServiceTests
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IValidatorManager> _validatorManagerMock = new Mock<IValidatorManager>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();

        public UserServiceTests()
        {
            _userService = new UserService(_repositoryManagerMock.Object, _mapperMock.Object, _configurationMock.Object, _validatorManagerMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ValidSpecilizations_ShouldCreateUser()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid specializationId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "email@email", Password = "password", UserRole = UserRole.RegularUser, SpecializationId = specializationId };
            var userDtoForCreate = new UserDtoForCreate("email@email", "password", specializationId, UserRole.RegularUser);
            var userDto = new UserDto(userId, "email@email", "password", UserRole.RegularUser, "adminSpecialization");
            var specialization = new Specialization { Id = specializationId, Name = "spec" };
            _mapperMock.Setup(m => m.Map<User>(userDtoForCreate)).Returns(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync(specialization);
            _repositoryManagerMock.Setup(r => r.UserRepository.Insert(user));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()));

            // Act
            var result = await _userService.CreateAsync(userDtoForCreate, It.IsAny<CancellationToken>());

            // Assert
            Assert.Equal(userDtoForCreate.Email, result.Email);
            _repositoryManagerMock.Verify(r => r.UserRepository.Insert(user), Times.Once);
            _repositoryManagerMock.Verify(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<User>(userDtoForCreate), Times.Once);
            _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_InvalidSpecializationId_ShouldThrowSpecializationNotFound()
        {
            // Arrange
            Guid specializationId = Guid.NewGuid();
            var userDtoForCreate = new UserDtoForCreate("email@email", "password", specializationId, UserRole.RegularUser);
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync((Specialization)null);

            // Act & Assert
            await Assert.ThrowsAsync<SpecializationNotFoundException>(() => _userService.CreateAsync(userDtoForCreate, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>();
            var usersDtos = new List<UserDto>();
            _repositoryManagerMock.Setup(r => r.UserRepository.GetAllUsersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(usersDtos);

            // Act
            var result = await _userService.GetAllAsync(It.IsAny<CancellationToken>());

            // Assert
            _repositoryManagerMock.Verify(r => r.UserRepository.GetAllUsersAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<UserDto>>(users), Times.Once);
            Assert.Equal(usersDtos.Count, result.Count);
        }

        [Fact]
        public async Task GetUserById_ValidUserId_ShouldReturnUser()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid specializationId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "email@email", Password = "password", UserRole = UserRole.RegularUser, SpecializationId = specializationId };
            var userDto = new UserDto(userId, "email@email", "password", UserRole.RegularUser, "adminSpecialization");
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetUserById(userId, It.IsAny<CancellationToken>());

            // Assert
            _repositoryManagerMock.Verify(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
            Assert.Equal(userDto.Email, result.Email);
        }

        [Fact]
        public async Task GetUserById_InvalidUserId_ShouldThrowUserNotFoundException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.GetUserById(userId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetUserByEmail_ValidUserId_ShouldReturnUser()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid specializationId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "email@email", Password = "password", UserRole = UserRole.RegularUser, SpecializationId = specializationId };
            var userDto = new UserDto(userId, "email@email", "password", UserRole.RegularUser, "adminSpecialization");
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByEmailAsync("email@email", It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetUserByEmail("email@email", It.IsAny<CancellationToken>());

            // Assert
            _repositoryManagerMock.Verify(r => r.UserRepository.GetUserByEmailAsync("email@email", It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
            Assert.Equal(userDto.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmail_InvalidUserId_ShouldThrowUserNotFoundEmailException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByEmailAsync("email@email", It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundEmailException>(() => _userService.GetUserByEmail("email@email", It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUserId_ShouldUpdateUser()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid specializationId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "email@email", Password = "password", SpecializationId = specializationId };
            var specialization = new Specialization { Id = specializationId, Name = "spec" };
            var userDtoForUpdate = new UserDtoForUpdate("email@email", "password", specializationId);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.SpecializationRepository.GetSpecializationByIdAsync(specializationId, It.IsAny<CancellationToken>())).ReturnsAsync(specialization);
            _mapperMock.Setup(m => m.Map(userDtoForUpdate, user));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _userService.UpdateAsync(userId, userDtoForUpdate, It.IsAny<CancellationToken>());

            // Assert
            _repositoryManagerMock.Verify(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map(userDtoForUpdate, user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_InvalidUserId_ShouldThrowUserNotFoundException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid specializationId = Guid.NewGuid();
            var userDtoForUpdate = new UserDtoForUpdate("email@email", "password", specializationId);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.UpdateAsync(userId, userDtoForUpdate, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DeleteUserAsync_ValidUserId_ShouldDeleteUser()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid specializationId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "email@email", Password = "password", SpecializationId = specializationId };
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repositoryManagerMock.Setup(r => r.UserRepository.Remove(user));

            // Act
            await _userService.DeleteAsync(userId, It.IsAny<CancellationToken>());

            // Assert
            _repositoryManagerMock.Verify(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryManagerMock.Verify(r => r.UserRepository.Remove(user), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_InvalidUserId_ShouldThrowUserNotFoundException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()));

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.DeleteAsync(userId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ValidateUserCredentials_ValidData_ShouldLogin()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid specializationId = Guid.NewGuid();
            string password = "password";
            byte[] salt;
            string hashedPassword = PasswordHasher.HashPassword(password, out salt);
            var user = new User { Id = userId, Email = "email@email", Password = hashedPassword, PasswordSalt = salt, SpecializationId = specializationId };
            var userDto = new UserDto(userId, "email@email", "password", UserRole.RegularUser, "adminSpecialization");
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByEmailAsync("email@email", It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.ValidateUserCredentials("email@email", "password", It.IsAny<CancellationToken>());

            // Assert
            _repositoryManagerMock.Verify(r => r.UserRepository.GetUserByEmailAsync("email@email", It.IsAny<CancellationToken>()));
            _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ValidateUserCredentials_InvalidEmail_ShouldThrowUserNotFoundEmailException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByEmailAsync("notExistingEmail@notExistingEmail", It.IsAny<CancellationToken>()));

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundEmailException>(() => _userService.ValidateUserCredentials("notExistingEmail@notExistingEmail", "password", It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ValidateUserCredentials_InvalidPassword_ShouldThrowUserCredentialsException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid specializationId = Guid.NewGuid();
            string password = "password";
            byte[] salt;
            string hashedPassword = PasswordHasher.HashPassword(password, out salt);
            var user = new User { Id = userId, Email = "email@email", Password = hashedPassword, PasswordSalt = salt, SpecializationId = specializationId };
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByEmailAsync("email@email", It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UserCredentialsException>(() => _userService.ValidateUserCredentials("email@email", "wrongPassword", It.IsAny<CancellationToken>()));
        }
    }
}
