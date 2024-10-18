using AutoMapper;
using Domain.RepositoryInterfaces;
using LikhodedDynamics.Sber.GigaChatSDK;
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
using EntityTask = Domain.Entities.Task;
using Contracts.Dtos.TaskDtos;
using Domain.Entities;
using FluentValidation;
using Contracts.Dtos.UserDtos;
using Domain.Exceptions.SpecializationExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.Exceptions.ProjectUsersExceptions;
using Domain.Exceptions.ColumnException;
using Domain.Exceptions.TaskExceptions;
using LikhodedDynamics.Sber.GigaChatSDK.Models;

namespace ProjectTests.ServiceTests
{
    public class TaskServiceTests
    {
        private readonly TaskService _taskService;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IValidatorManager> _validatorManagerMock = new Mock<IValidatorManager>();
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<GigaChat> _gigaChatMock = new Mock<GigaChat>("", false, true, false);

        private readonly static Guid taskId = Guid.NewGuid();
        private readonly static Guid userId = Guid.NewGuid();
        private readonly static Guid columnId = Guid.NewGuid();
        private readonly static Guid projectId = Guid.NewGuid();
        private readonly static Guid specializationId = Guid.NewGuid();
        private readonly static DateTime dateCreated = DateTime.UtcNow;
        private readonly static DateTime dateEnd = dateCreated.AddMinutes(10);
        private readonly EntityTask task = new EntityTask { Id = taskId, Title = "taskTitle", ColumnId = columnId, DateCreated = dateCreated, DateEnd = dateEnd, Priority = Domain.Entities.Enums.Priority.Medium, Status = Domain.Entities.Enums.Status.OnQueue, Description = "description" };
        private readonly TaskDto taskDto = new TaskDto(taskId, "taskTitle", "description", Domain.Entities.Enums.Priority.Medium, Domain.Entities.Enums.Status.OnQueue, dateCreated, dateEnd, userId, columnId);
        private readonly TaskDtoForCreate taskDtoForCreate = new TaskDtoForCreate("taskTitle", "description", Domain.Entities.Enums.Priority.Medium, dateEnd, userId, columnId);
        private readonly TaskDtoForUpdate taskDtoForUpdate = new TaskDtoForUpdate("newTitle", "newDescription", null, null, null, userId, columnId);
        private readonly User user = new User { Id = userId, Email = "email@email", Password = "password", SpecializationId = specializationId};
        private readonly Column column = new Column { Id = columnId, Title = "columnTitle", ProjectId = projectId };
        private readonly Project project = new Project { Id = projectId, Name = "proj"};
        private readonly ProjectUsers projectUsers = new ProjectUsers { UserId = userId, ProjectId = projectId, RoleOnProject = Domain.Entities.Enums.RoleOnProject.TeamLead };
        private readonly CancellationToken token = It.IsAny<CancellationToken>();

        public TaskServiceTests()
        {
            _taskService = new TaskService(_repositoryManagerMock.Object, _mapperMock.Object, _validatorManagerMock.Object, _notificationServiceMock.Object, _configurationMock.Object, _gigaChatMock.Object);
        }

        [Fact]
        public async Task CreateTaskAsync_ValidData_ShouldCreateTask()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForCreate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token)).ReturnsAsync(projectUsers);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync(column);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(projectId, token)).ReturnsAsync(project);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(column.ProjectId, token)).ReturnsAsync(project);
            _mapperMock.Setup(m => m.Map<EntityTask>(taskDtoForCreate)).Returns(task);
            _repositoryManagerMock.Setup(r => r.TaskRepository.Insert(task));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));
            _notificationServiceMock.Setup(n => n.SendAsync("toEmail", "subject", "body", token)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TaskDto>(task)).Returns(taskDto);

            // Act
            var result = await _taskService.CreateAsync(projectId, taskDtoForCreate, token);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskDto>(result);
            Assert.Equal(taskDtoForCreate.Title, result.Title);
        }

        [Fact]
        public async Task CreateTaskAsync_InvalidData_ShouldThrowValidationException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForCreate, token)).ThrowsAsync(new ValidationException(""));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _taskService.CreateAsync(projectId, taskDtoForCreate, token));
        }

        [Fact]
        public async Task CreateTaskAsync_InvalidUserId_ShouldThrowUserNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForCreate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _taskService.CreateAsync(projectId, taskDtoForCreate, token));
        }

        [Fact]
        public async Task CreateTaskAsync_UserNotInProject_ShouldThrowProjectUsersNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForCreate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token)).ReturnsAsync((ProjectUsers)null);

            // Act & Assert
            await Assert.ThrowsAsync<ProjectUsersNotFoundException>(() => _taskService.CreateAsync(projectId, taskDtoForCreate, token));
        }

        [Fact]
        public async Task CreateTaskAsync_InvalidColumnId_ShouldThrowColumnNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForCreate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token)).ReturnsAsync(projectUsers);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(columnId, token)).ReturnsAsync((Column)null);

            // Act & Assert
            await Assert.ThrowsAsync<ColumnNotFoundException>(() => _taskService.CreateAsync(projectId, taskDtoForCreate, token));
        }

        [Fact]
        public async Task CreateTaskAsync_ColumnDependencyError_ShouldThrowTaskCreatingErrorWithColumnDependency()
        {
            // Arrange
            Guid otherProjectColumnId = Guid.NewGuid();
            Guid otherProjectId = Guid.NewGuid();
            Project otherProject = new Project { Id = otherProjectId, Name = "otherProj"};
            Column otherProjectColumn = new Column { Id = otherProjectColumnId, Title = "otherTitle", ProjectId = otherProjectId };
            TaskDtoForCreate taskDtoForCreate = new TaskDtoForCreate("taskTitle", "description", Domain.Entities.Enums.Priority.Medium, dateEnd, userId, otherProjectColumnId);
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForCreate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.ProjectUsersRepository.GetProjectUser(userId, projectId, token)).ReturnsAsync(projectUsers);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(otherProjectColumnId, token)).ReturnsAsync(otherProjectColumn);
            _repositoryManagerMock.Setup(r => r.ProjectRepository.GetProjectByIdAsync(otherProjectId, token)).ReturnsAsync(otherProject);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCreatingErrorWithColumnDependency>(() => _taskService.CreateAsync(projectId, taskDtoForCreate, token));
        }

        [Fact]
        public async Task GetAllTasksAsync_ShouldReturnListOfTasksDto()
        {
            // Arrange
            List<EntityTask> tasks = new List<EntityTask> { task };
            List<TaskDto> tasksDtos = new List<TaskDto> { taskDto };
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetAllTasksAsync(token)).ReturnsAsync(tasks);
            _mapperMock.Setup(m => m.Map<List<TaskDto>>(tasks)).Returns(tasksDtos);

            // Act
            var result = await _taskService.GetAllAsync(token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tasks.Count, result.Count);
            Assert.IsType<TaskDto>(result[0]);
        }

        [Fact]
        public async Task GetAllTasksByUser_ValidUserId_ShouldReturnListOfTasksDto()
        {
            // Arrange
            List<EntityTask> tasks = new List<EntityTask> { task };
            List<TaskDto> tasksDtos = new List<TaskDto> { taskDto };
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetAllTasksForUserAsync(userId, token)).ReturnsAsync(tasks);
            _mapperMock.Setup(m => m.Map<List<TaskDto>>(tasks)).Returns(tasksDtos);

            // Act
            var result = await _taskService.GetAllTasksForUserAsync(userId, token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tasks.Count, result.Count);
            Assert.IsType<TaskDto>(result[0]);
        }


        [Fact]
        public async Task GetAllTasksByUser_InvalidUserId_ShouldThrowUserNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync(userId, token)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _taskService.GetAllTasksForUserAsync(userId, token));
        }

        [Fact]
        public async Task UpdateTask_ValidData_ShouldUpdateTask()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForUpdate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetTaskByIdAsync(taskId, token)).ReturnsAsync(task);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync((Guid)taskDtoForUpdate.UserId, token)).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync((Guid)taskDtoForUpdate.ColumnId, token)).ReturnsAsync(column);
            _mapperMock.Setup(m => m.Map(taskDtoForUpdate, task)).Returns(task);
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));

            // Act
            await _taskService.UpdateAsync(taskId, taskDtoForUpdate, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_InvalidData_ShouldThrowValidationException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForUpdate, token)).ThrowsAsync(new ValidationException(""));


            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _taskService.UpdateAsync(userId, taskDtoForUpdate, token));
        }

        [Fact]
        public async Task UpdateTask_InvalidTaskId_ShouldThrowTaskNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForUpdate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetTaskByIdAsync(taskId, token)).ReturnsAsync((EntityTask)null);

            // Act & Assert
            await Assert.ThrowsAsync<TaskNotFoundException>(() => _taskService.UpdateAsync(userId, taskDtoForUpdate, token));
        }

        [Fact]
        public async Task UpdateTask_InvalidUserId_ShouldThrowUserNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForUpdate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetTaskByIdAsync(taskId, token)).ReturnsAsync(task);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync((Guid)taskDtoForUpdate.UserId, token)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _taskService.UpdateAsync(taskId, taskDtoForUpdate, token));
        }

        [Fact]
        public async Task UpdateTask_InvalidColumnId_ShouldThrowColumnNotFoundException()
        {
            // Arrange
            _validatorManagerMock.Setup(v => v.ValidateAsync(taskDtoForUpdate, token)).Returns(Task.CompletedTask);
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetTaskByIdAsync(taskId, token)).ReturnsAsync(task);
            _repositoryManagerMock.Setup(r => r.UserRepository.GetUserByIdAsync((Guid)taskDtoForUpdate.UserId, token)).ReturnsAsync(user);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync((Guid)taskDtoForUpdate.ColumnId, token)).ReturnsAsync((Column)null);

            // Act & Assert
            await Assert.ThrowsAsync<ColumnNotFoundException>(() => _taskService.UpdateAsync(taskId, taskDtoForUpdate, token));
        }

        [Fact]
        public async Task DeleteTask_ValidTaskId_ShouldDeleteTask()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetTaskByIdAsync(taskId, token)).ReturnsAsync(task);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(task.ColumnId, token)).ReturnsAsync(column);
            _repositoryManagerMock.Setup(r => r.TaskRepository.Remove(task));
            _repositoryManagerMock.Setup(r => r.UnitOfWork.SaveChangesAsync(token));

            // Act
            await _taskService.DeleteAsync(projectId, taskId, token);

            // Assert
            _repositoryManagerMock.Verify(r => r.TaskRepository.Remove(task), Times.Once);
            _repositoryManagerMock.Verify(r => r.UnitOfWork.SaveChangesAsync(token), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_InvalidTaskId_ShouldThrowTaskNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetTaskByIdAsync(taskId, token)).ReturnsAsync((EntityTask)null);

            // Act & Assert
            await Assert.ThrowsAsync<TaskNotFoundException>(() => _taskService.DeleteAsync(projectId, taskId, token));
        }

        [Fact]
        public async Task DeleteTask_InvalidColumnId_ShouldThrowColumnNotFoundException()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetTaskByIdAsync(taskId, token)).ReturnsAsync(task);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(task.ColumnId, token)).ReturnsAsync((Column)null);

            // Act & Assert
            await Assert.ThrowsAsync<ColumnNotFoundException>(() => _taskService.DeleteAsync(projectId, taskId, token));
        }

        [Fact]
        public async Task DeleteTask_InvalidProjectId_ShouldTaskCreatingErrorWithColumnDependency()
        {
            // Arrange
            _repositoryManagerMock.Setup(r => r.TaskRepository.GetTaskByIdAsync(taskId, token)).ReturnsAsync(task);
            _repositoryManagerMock.Setup(r => r.ColumnRepository.GetColumnByIdAsync(task.ColumnId, token)).ReturnsAsync(column);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCreatingErrorWithColumnDependency>(() => _taskService.DeleteAsync(Guid.NewGuid(), taskId, token));
        }
    }
}
