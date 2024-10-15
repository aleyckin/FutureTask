using AutoMapper;
using Contracts.Dtos.TaskDtos;
using Contracts.Dtos.UserDtos;
using Domain.Exceptions.ColumnException;
using Domain.Exceptions.ProjectUsersExceptions;
using Domain.Exceptions.TaskExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.RepositoryInterfaces;
using LikhodedDynamics.Sber.GigaChatSDK;
using LikhodedDynamics.Sber.GigaChatSDK.Models;
using Microsoft.Extensions.Configuration;
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
        private static readonly bool MustCreateChatBot = true;
        private readonly GigaChat _chat;

        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public TaskService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager, INotificationService notificationService, IConfiguration configuration, GigaChat chat)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
            _notificationService = notificationService;
            _configuration = configuration;
            _chat = chat;
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

        public async Task<string> GetResponseByChatBot(Guid taskId, string userMessage, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync((Guid)taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            if (_chat.Token == null)
            {
                await _chat.CreateTokenAsync();
            }

            if (task.ContextMessages == null || task.ContextMessages.Count == 0)
            {
                var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(task.ColumnId, cancellationToken);
                var project = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(column.ProjectId, cancellationToken);

                string taskInfo = $"Название проекта: {project.Name} " +
                    $"\n Уточнение тематики: {column.Title} " +
                    $"\n Название задачи, которую нужно решить: {task.Title} " +
                    $"\n Описание задачи: {task.Description}." +
                    $"\n Дополнительные требования/объяснения: {userMessage}.";

                var response = await _chat.CompletionsAsync(taskInfo);

                string stringResponse = response.choices.LastOrDefault().message.content;

                task.ContextMessages = new List<string> { taskInfo };
                task.Conversation = new List<string> { taskInfo, stringResponse };
                await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

                return stringResponse;
            }         

            MessageQuery messageQuery = new MessageQuery();
            MessageContent messageContent;
            foreach (var message in task.ContextMessages)
            {
                messageContent = new MessageContent("user", message);
                messageQuery.messages.Add(messageContent);
            }
            messageContent = new MessageContent("user", userMessage);
            messageQuery.messages.Add(messageContent);

            Response? responseBig = await _chat.CompletionsAsync(messageQuery);
            string stringResponseBig = responseBig.choices.LastOrDefault().message.content;

            task.ContextMessages.Add(userMessage);
            task.Conversation.Add(userMessage);
            task.Conversation.Add(stringResponseBig);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return stringResponseBig;
        }

        public async Task<List<string>> GetConversation(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            if (task.Conversation == null || task.Conversation.Count == 0)
            {
                throw new ChatBotContextException(taskId);
            }

            return task.Conversation;
        }

        public async Task<List<string>> GetTaskChatBotContext(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            if (task.ContextMessages == null || task.ContextMessages.Count == 0)
            {
                throw new ChatBotContextException(taskId);
            }

            return task.ContextMessages;
        }

        public async Task DeleteTaskChatBotContext(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _repositoryManager.TaskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }
            
            task.ContextMessages = null;
            task.Conversation = null;
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
