using AutoMapper;
using Contracts.Dtos.ColumnDtos;
using Contracts.Dtos.ProjectDtos;
using Domain.Entities;
using Domain.Exceptions.ProjectExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.RepositoryInterfaces;
using FluentValidation;
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
    public class ProjectServiceTests
    {
        private readonly ProjectService _projectService;
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IValidatorManager> _validatorManagerMock = new Mock<IValidatorManager>();

        readonly static Guid projectId = Guid.NewGuid();
        readonly static List<ColumnDto> columnsList = new List<ColumnDto>();
        readonly Project project = new Project { Name = "proj" };
        readonly ProjectDto projectDto = new ProjectDto(projectId, "proj", columnsList);
        readonly ProjectDtoForCreate projectDtoForCreate = new ProjectDtoForCreate("proj");
        readonly ProjectDtoForUpdate projectDtoForUpdate = new ProjectDtoForUpdate("newName");
        readonly CancellationToken token = It.IsAny<CancellationToken>();

        public ProjectServiceTests()
        {
            _projectService = new ProjectService(_repositoryManagerMock.Object, _mapperMock.Object, _validatorManagerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidData_ShouldCreateProject()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectDtoForCreate, token)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<Project>(projectDtoForCreate)).Returns(project);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.Insert(project));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));
            _mapperMock.Setup(m => m.Map<ProjectDto>(project)).Returns(projectDto);

            // Act
            var result = await _projectService.CreateAsync(projectDtoForCreate, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ProjectRepository.Insert(project), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
            Assert.Equal(projectDtoForCreate.Name, result.Name);
            Assert.IsAssignableFrom<ProjectDto>(result);
        }

        [Fact]
        public async Task CreateAsync_InvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var projectDtoForCreate = new ProjectDtoForCreate(null);
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectDtoForCreate, token)).ThrowsAsync(new ValidationException("Validation error"));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _projectService.CreateAsync(projectDtoForCreate, token));
        }


        [Fact]
        public async Task GetAllProjects_ShouldReturnListOfProjects()
        {
            // Arrange
            List<Project> projects = new List<Project> { project };
            List<ProjectDto> projectsDtos = new List<ProjectDto> { projectDto };
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetAllProjectsAsync(token)).ReturnsAsync(projects);
            _mapperMock.Setup(m => m.Map<List<ProjectDto>>(projects)).Returns(projectsDtos);

            // Act
            var result = await _projectService.GetAllAsync(token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ProjectRepository.GetAllProjectsAsync(token), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(projects.Count, result.Count);
            Assert.IsAssignableFrom<ProjectDto>(result[0]);
        }

        [Fact]
        public async Task GetProjectById_ValidProjectId_ShouldReturnProject()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);
            _mapperMock.Setup(m => m.Map<ProjectDto>(project)).Returns(projectDto);

            // Act
            var result = await _projectService.GetProjectById(projectId, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token), Times.Once);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ProjectDto>(result);
            Assert.Equal(project.Name, result.Name);
        }

        [Fact]
        public async Task GetProjectById_InvalidProjectId_ShouldThrowProjectNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProjectNotFoundException>(() => _projectService.GetProjectById(projectId, token));
        }

        [Fact]
        public async Task UpdateProjectById_ValidProjectId_ShouldUpdateProject()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectDtoForUpdate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);
            _mapperMock.Setup(m => m.Map(projectDtoForUpdate, project)).Returns(project);
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));

            // Act
            await _projectService.UpdateAsync(projectId, projectDtoForUpdate, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task UpdateProjectById_InvalidProjectId_ShouldThrowProjectNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync((Project)null);

            // Act & // Assert
            await Assert.ThrowsAsync<ProjectNotFoundException>(() => _projectService.GetProjectById(projectId, token));
        }

        [Fact]
        public async Task UpdateProjectById_InvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var projectDtoForUpdate = new ProjectDtoForUpdate(null);
            _validatorManagerMock.Setup(v => v.ValidateAsync(projectDtoForUpdate, token)).ThrowsAsync(new ValidationException("Validation error"));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _projectService.UpdateAsync(projectId, projectDtoForUpdate, token));
        }

        [Fact]
        public async Task DeleteProjectById_ValidProjectId_ShouldDeleteProject()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.Remove(project));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));

            // Act
            await _projectService.DeleteAsync(projectId, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token), Times.Once);
            _repositoryManagerMock.Verify(r => r.ProjectRepository.Remove(project), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task DeleteProjectById_InvalidProjectId_ShouldThrowProjectNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProjectNotFoundException>(() => _projectService.GetProjectById(projectId, token));
        }
    }
}
