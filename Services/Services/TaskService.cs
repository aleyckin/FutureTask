using AutoMapper;
using Contracts.Dtos.TaskDtos;
using Contracts.Dtos.UserDtos;
using Domain.Exceptions.TaskExceptions;
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

        public TaskService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<TaskDto> CreateAsync(TaskDtoForCreate taskDtoForCreate, CancellationToken cancellationToken = default)
        {
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

        public async Task<TaskDto> GetTaskById(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            return _mapper.Map<TaskDto>(task);
        }

        public async Task UpdateAsync(Guid taskId, TaskDtoForUpdate taskDtoForUpdate, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }
            if (taskDtoForUpdate.Title != null)
            {
                task.Title = taskDtoForUpdate.Title;
            }
            if (taskDtoForUpdate.Description != null)
            {
                task.Description = taskDtoForUpdate.Description;
            }
            if (taskDtoForUpdate.Priority != null)
            {
                task.Priority = (Domain.Entities.Enums.Priority)taskDtoForUpdate.Priority;
            }
            if (taskDtoForUpdate.Status != null)
            {
                task.Status = (Domain.Entities.Enums.Status)taskDtoForUpdate.Status;
            }
            if (taskDtoForUpdate.DateEnd != null)
            {
                task.DateEnd = (DateTime)taskDtoForUpdate.DateEnd;
            }
            if (taskDtoForUpdate.UserId != null)
            {
                task.UserId = (Guid)taskDtoForUpdate.UserId;
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
