using AutoMapper;
using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.ProjectUsersDtos;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Exceptions.ProjectExceptions;
using Domain.Exceptions.ProjectUsersExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.RepositoryInterfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Moq;
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
    public class ProjectUsersServiceTests
    {
        private readonly ProjectUsersService _projectUserService;

        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IValidatorManager> _validatorManagerMock = new Mock<IValidatorManager>();

        private readonly static Guid userId = Guid.NewGuid();
        private readonly static Guid projectId = Guid.NewGuid();
        private readonly User user = new User { Id = userId, Email = "email@email" };
        private readonly Project project = new Project { Id = projectId, Name = "proj" };
        private readonly ProjectUsers projectUser = new ProjectUsers { UserId = userId, ProjectId = projectId, RoleOnProject = Domain.Entities.Enums.RoleOnProject.DefaultWorker };
        private readonly ProjectUsersDto projectUsersDto = new ProjectUsersDto(userId, projectId, Domain.Entities.Enums.RoleOnProject.DefaultWorker);
        readonly CancellationToken token = It.IsAny<CancellationToken>();

        public ProjectUsersServiceTests() 
        {
            _projectUserService = new ProjectUsersService(_repositoryManagerMock.Object, _mapperMock.Object, _validatorManagerMock.Object);
        }

        [Fact]
        public async Task AddUserToProjectAsync_ValidData_ShouldAddUserToProject()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectUsersDto, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.Insert(projectUser));
            _mapperMock.Setup(m => m.Map<ProjectUsers>(projectUsersDto)).Returns(projectUser);
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));

            // Act
            await _projectUserService.AddUserToProjectAsync(projectUsersDto, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ProjectUsersRepository.Insert(projectUser), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task AddUserToProjectAsync_InvalidUserId_ShouldThrowUserNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectUsersDto, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _projectUserService.AddUserToProjectAsync(projectUsersDto, token));
        }

        [Fact]
        public async Task AddUserToProjectAsync_InvalidProjectId_ShouldThrowProjectNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectUsersDto, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProjectNotFoundException>(() => _projectUserService.AddUserToProjectAsync(projectUsersDto, token));
        }

        [Fact]
        public async Task AddUserToProjectAsync_InvalidRoleData_ShouldThrowValidationException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectUsersDto, token)).ThrowsAsync(new ValidationException("Validation error"));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _projectUserService.AddUserToProjectAsync(projectUsersDto, token));
        }

        [Fact]
        public async Task GetAllProjectsByUserAsync_ValidUserId_ShouldReturnListOfProjectUsers()
        {
            // Arrange
            List<ProjectUsersDtoForListProjects> projectDtoList = new List<ProjectUsersDtoForListProjects> { new ProjectUsersDtoForListProjects("deleteMe", Domain.Entities.Enums.RoleOnProject.DefaultWorker) };
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync(user);
            List<ProjectUsers> projectUsers = new List<ProjectUsers> { projectUser };
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetAllProjectsByUser(userId, token)).ReturnsAsync(projectUsers);
            _mapperMock.Setup(m => m.Map<List<ProjectUsersDtoForListProjects>>(projectUsers)).Returns(projectDtoList);

            // Act
            var result = await _projectUserService.GetAllProjectsByUser(userId, token);

            // Assert
            Assert.IsAssignableFrom<ProjectUsersDtoForListProjects>(result[0]);
            _repositoryManagerMock.Verify(r => r.UserRepository.GetUserByIdAsync(userId, token), Times.Once);
            _repositoryManagerMock.Verify(r => r.ProjectUsersRepository.GetAllProjectsByUser(userId, token), Times.Once);
        }

        [Fact]
        public async Task GetAllProjectsByUserAsync_InvalidUserId_ShouldThrowUserNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _projectUserService.GetAllProjectsByUser(userId, token));
        }

        [Fact]
        public async Task GetAllUsersByProjectAsync_ValidProjectId_ShouldReturnListOfUsers()
        {
            // Arrange
            List<ProjectUsersDtoForListUsers> projectUsersDtoList = new List<ProjectUsersDtoForListUsers> { new ProjectUsersDtoForListUsers("email@email", Domain.Entities.Enums.RoleOnProject.DefaultWorker) };
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);
            List<ProjectUsers> projectUsers = new List<ProjectUsers> { projectUser };
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetAllUsersByProject(projectId, token)).ReturnsAsync(projectUsers);
            _mapperMock.Setup(m => m.Map<List<ProjectUsersDtoForListUsers>>(projectUsers)).Returns(projectUsersDtoList);

            // Act
            var result = await _projectUserService.GetAllUsersByProject(projectId, token);

            // Assert
            Assert.IsAssignableFrom<ProjectUsersDtoForListUsers>(result[0]);
            _repositoryManagerMock.Verify(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token), Times.Once);
            _repositoryManagerMock.Verify(r => r.ProjectUsersRepository.GetAllUsersByProject(projectId, token), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersByProjectAsync_InvalidProjectId_ShouldThrowProjectNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProjectNotFoundException>(() => _projectUserService.GetAllUsersByProject(projectId, token));
        }

        [Fact]
        public async Task UpdateUserRoleInProject_ValidData_ShouldUpdateProjectUser()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectUsersDto, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token)).ReturnsAsync(projectUser);
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));

            // Act
            await _projectUserService.UpdateUserRoleInProjectAsync(projectUsersDto, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task UpdateUserRoleInProject_InvalidData_ShouldThrowProjectUsersNotFound()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token)).ReturnsAsync((ProjectUsers)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProjectUsersNotFoundException>(() => _projectUserService.UpdateUserRoleInProjectAsync(projectUsersDto, token));
        }

        [Fact]
        public async Task DeleteUserFromProjectAsync_ValidData_ShouldDeleteProjectUsers()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(projectUsersDto.UserId, projectUsersDto.ProjectId, token)).ReturnsAsync(projectUser);
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.Remove(projectUser));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));

            // Act
            await _projectUserService.DeleteUserFromProjectAsync(projectUsersDto, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ProjectUsersRepository.Remove(projectUser), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task DeleteUserFromProjectAsync_InvalidData_ShouldThrowProjectUsersNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(projectUsersDto.UserId, projectUsersDto.ProjectId, token)).ReturnsAsync((ProjectUsers)null);

            // Act & Assert 
            await Assert.ThrowsAsync<ProjectUsersNotFoundException>(() => _projectUserService.DeleteUserFromProjectAsync(projectUsersDto, token));
        }

        [Fact]
        public async Task GetUserRoleOnProject_ValidData_ShouldReturnRoleOnProject()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token)).ReturnsAsync(projectUser);

            // Act
            var result = await _projectUserService.GetUserRoleOnProject(userId, projectId, token);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RoleOnProject>(result);
        }

        [Fact]
        public async Task GetUserRoleOnProject_InvalidData_ShouldThrowProjectUsersNotFound()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token)).ReturnsAsync((ProjectUsers)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProjectUsersNotFoundException>(() => _projectUserService.GetUserRoleOnProject(userId, projectId, token));
        }
    }
}
