using AutoMapper;
using Contracts.Dtos.ColumnDtos;
using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.TaskDtos;
using Domain.Entities;
using Domain.Exceptions.ColumnException;
using Domain.Exceptions.ColumnExceptions;
using Domain.Exceptions.ProjectExceptions;
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
    public class ColumnServiceTests
    {
        private readonly ColumnService _columnService;
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IValidatorManager> _validatorManagerMock = new Mock<IValidatorManager>();

        readonly static Guid columnId = Guid.NewGuid();
        readonly static Guid projectId = Guid.NewGuid();
        readonly static List<TaskDto> tasksDtosList = new List<TaskDto>();
        readonly Column column = new Column { Title = "proj", ProjectId = projectId };
        readonly ColumnDto columnDto = new ColumnDto(columnId, "title", projectId, tasksDtosList);
        readonly ColumnDtoForCreate columnDtoForCreate = new ColumnDtoForCreate("title", projectId);
        readonly ColumnDtoForUpdate columnDtoForUpdate = new ColumnDtoForUpdate("newTitle");
        readonly Project project = new Project { Id = projectId, Name = "proj" };
        readonly CancellationToken token = It.IsAny<CancellationToken>();

        public ColumnServiceTests()
        {
            _columnService = new ColumnService(_repositoryManagerMock.Object, _mapperMock.Object, _validatorManagerMock.Object);
        }

        [Fact]
        public async Task CreateColumnAsync_ValidData_ShouldCreateColumn()
        {
            // Arrange
            _validatorManagerMock.Setup(r => r.ValidateAsync(columnDtoForCreate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);
            _mapperMock.Setup(m => m.Map<Column>(columnDtoForCreate)).Returns(column);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.Insert(column));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));
            _mapperMock.Setup(m => m.Map<ColumnDto>(column)).Returns(columnDto);

            // Act
            var result = await _columnService.CreateAsync(projectId, columnDtoForCreate, token);

            // Assert
            Assert.Equal(columnDtoForCreate.Title, result.Title);
            Assert.NotNull(result);
            Assert.IsType<ColumnDto>(result);
        }

        [Fact]
        public async Task CreateColumnAsync_InvalidProjectId_ShouldThrowProjectNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(r => r.ValidateAsync(columnDtoForCreate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync((Project)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProjectNotFoundException>(() => _columnService.CreateAsync(projectId, columnDtoForCreate, token));
        }

        [Fact]
        public async Task CreateColumnAsync_DifferenceBetweenProjectId_ShouldThrowColumnCreatingErrorWithProjectDependencyException()
        {
            // Arrange
            _validatorManagerMock.Setup(r => r.ValidateAsync(columnDtoForCreate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);

            // Act & Assert
            await Assert.ThrowsAsync<ColumnCreatingErrorWithProjectDependency>(() => _columnService.CreateAsync(Guid.NewGuid(), columnDtoForCreate, token));
        }

        [Fact]
        public async Task CreateColumnAsync_InvalidData_ShouldThrowValidationException()
        {
            // Arrange
            _validatorManagerMock.Setup(r => r.ValidateAsync(columnDtoForCreate, token)).ThrowsAsync(new ValidationException(""));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _columnService.CreateAsync(projectId, columnDtoForCreate, token));
        }

        [Fact]
        public async Task GetAllColumns_ShouldReturnListOfColumns()
        {
            // Arrange
            List<Column> columns = new List<Column> { column };
            List<ColumnDto> columnsDtos = new List<ColumnDto> { columnDto };
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetAllColumnsAsync(token)).ReturnsAsync(columns);
            _mapperMock.Setup(m => m.Map<List<ColumnDto>>(columns)).Returns(columnsDtos);

            // Act
            var result = await _columnService.GetAllAsync(token);

            // Assert
            Assert.Equal(columnDtoForCreate.Title, result[0].Title);
            Assert.IsType<ColumnDto>(result[0]);
        }

        [Fact]
        public async Task GetColumnById_ValidColumnId_ShouldReturnColumn()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync(column);
            _mapperMock.Setup(m => m.Map<ColumnDto>(column)).Returns(columnDto);

            // Act
            var result = await _columnService.GetColumnById(columnId, token);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ColumnDto>(result);
        }

        [Fact]
        public async Task GetColumnById_InvalidColumnId_ShouldThrowColumnNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync((Column)null);

            // Act & Assert
            await Assert.ThrowsAsync<ColumnNotFoundException>(() => _columnService.GetColumnById(columnId, token));
        }

        [Fact]
        public async Task UpdateColumnById_ValidData_ShouldUpdateColumn()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(columnDtoForUpdate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync(column);
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));
            _mapperMock.Setup(m => m.Map(columnDtoForUpdate, column)).Returns(column);

            // Act
            await _columnService.UpdateAsync(columnId, columnDtoForUpdate, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task UpdateColumnById_InvalidData_ShouldThrowValidationException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(columnDtoForUpdate, token)).ThrowsAsync(new ValidationException(""));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _columnService.UpdateAsync(columnId, columnDtoForUpdate, token));
        }

        [Fact]
        public async Task UpdateColumnById_InvalidColumnId_ShouldThrowColumnNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(columnDtoForUpdate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync((Column)null);

            // Act & Assert
            await Assert.ThrowsAsync<ColumnNotFoundException>(() => _columnService.UpdateAsync(columnId, columnDtoForUpdate, token));
        }

        [Fact]
        public async Task DeleteColumnById_ValidColumnId_ShouldDeleteColumn()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync(column);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.Remove(column));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));

            // Act
            await _columnService.DeleteAsync(projectId, columnId, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.ColumnRepository.Remove(column), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task DeleteColumnById_InvalidColumnId_ShouldThrowColumnNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync((Column)null);

            // Act & Assert
            await Assert.ThrowsAsync<ColumnNotFoundException>(() => _columnService.DeleteAsync(projectId, columnId, token));
        }

        [Fact]
        public async Task DeleteColumnById_DifferenceProjectId_ShouldThrowColumnNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync(column);

            // Act & Assert
            await Assert.ThrowsAsync<ColumnCreatingErrorWithProjectDependency>(() => _columnService.DeleteAsync(Guid.NewGuid(), columnId, token));
        }
    }
}
