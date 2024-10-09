using AutoMapper;
using Contracts.Dtos.TaskDtos;
using Contracts.Dtos.UserDtos;
using Domain.Exceptions.ColumnException;
using Domain.Exceptions.ProjectUsersExceptions;
using Domain.Exceptions.TaskExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.RepositoryInterfaces;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;
        private readonly INotificationService _notificationService;

        public TaskService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager, INotificationService notificationService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
            _notificationService = notificationService;
        }

        public async Task<TaskDto> CreateAsync(Guid projectId, TaskDtoForCreate taskDtoForCreate, CancellationToken cancellationToken = default)
        {
            //Валидация данных
            await _validatorManager.ValidateAsync(taskDtoForCreate, cancellationToken);

            //Проверяем существует ли вообще юзер, {Id} которого передали
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(taskDtoForCreate.UserId, cancellationToken);
            if (user == null) 
            {
                throw new UserNotFoundException(taskDtoForCreate.UserId);
            }

            //Проверяем приписан ли человек к проекту
            var projectUser = await _repositoryManager.ProjectUsersRepository.GetProjectUser(taskDtoForCreate.UserId, projectId, cancellationToken);
            if (projectUser == null)
            {
                throw new ProjectUsersNotFoundException(taskDtoForCreate.UserId, projectId);
            }

            //Проверяем существует ли вообще колонка, {Id} которой передали
            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(taskDtoForCreate.ColumnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(taskDtoForCreate.ColumnId);
            }

            //Проверяем принадлежит ли колонка, в которую мы пытаемся добавить Task, тому же проекту, в котором мы сейчас находимся(!!)
            var CheckColumnOnProject = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(projectId, cancellationToken);
            var ColumnProject = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(column.ProjectId, cancellationToken);
            if (CheckColumnOnProject != ColumnProject)
            {
                throw new TaskCreatingErrorWithColumnDependency(taskDtoForCreate.ColumnId, projectId);
            }

            var task = _mapper.Map<Domain.Entities.Task>(taskDtoForCreate);
            task.DateEnd = DateTime.SpecifyKind(taskDtoForCreate.DateEnd, DateTimeKind.Utc);
            _repositoryManager.TaskRepository.Insert(task);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            string subject = "New Task Created";
            string body = $"Dear {user.Email},\n\nA new task has been created for you: {task.Title}";
            await _notificationService.SendAsync(user.Email, subject, body, cancellationToken);

            return _mapper.Map<TaskDto>(task);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(task.ColumnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(task.ColumnId);
            }

            if (projectId != column.ProjectId)
            {
                throw new TaskCreatingErrorWithColumnDependency(task.ColumnId, projectId);
            }

            _repositoryManager.TaskRepository.Remove(task);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<TaskDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var tasks = await _repositoryManager.TaskRepository.GetAllTasksAsync(cancellationToken);
            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<List<TaskDto>> GetAllTasksForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var tasks = await _repositoryManager.TaskRepository.GetAllTasksForUserAsync(userId, cancellationToken);
            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<TaskDto> GetTaskById(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }
            return _mapper.Map<TaskDto>(task);
        }

        public async Task UpdateAsync(Guid taskId, TaskDtoForUpdate taskDtoForUpdate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(taskDtoForUpdate, cancellationToken);

            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            var user = await _repositoryManager.UserRepository.GetUserByIdAsync((Guid)taskDtoForUpdate.UserId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException((Guid)taskDtoForUpdate.UserId);
            }

            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync((Guid)taskDtoForUpdate.ColumnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException((Guid)taskDtoForUpdate.ColumnId);
            }

            _mapper.Map(taskDtoForUpdate, task);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
