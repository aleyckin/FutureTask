using AutoMapper;
using Contracts.Dtos.TaskDtos;
using Contracts.Dtos.UserDtos;
using Domain.Exceptions.ColumnException;
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

        public TaskService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async Task<TaskDto> CreateAsync(TaskDtoForCreate taskDtoForCreate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(taskDtoForCreate, cancellationToken);

            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(taskDtoForCreate.UserId, cancellationToken);
            if (user == null) 
            {
                throw new UserNotFoundException(taskDtoForCreate.UserId);
            }

            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(taskDtoForCreate.ColumnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(taskDtoForCreate.ColumnId);
            }

            var task = _mapper.Map<Domain.Entities.Task>(taskDtoForCreate);
            task.DateEnd = DateTime.SpecifyKind(taskDtoForCreate.DateEnd, DateTimeKind.Utc);
            _repositoryManager.TaskRepository.Insert(task);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<TaskDto>(task);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
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
