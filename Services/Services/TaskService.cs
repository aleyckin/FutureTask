using AutoMapper;
using Contracts.Dtos.MessageDtos;
using Contracts.Dtos.TaskDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities.Enums;
using Domain.Entities.Helpers;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Services
{
    public class TaskService : ITaskService
    {
        private readonly GigaChat _chat;

        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectUsersRepository _projectUsersRepository;
        private readonly IColumnRepository _columnRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public TaskService(ITaskRepository taskRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IColumnRepository columnRepository,
            IProjectUsersRepository projectUsersRepository,
            IUnitOfWork unitOfWork,
            IMessageRepository messageRepository,
            IMapper mapper,
            IValidatorManager validatorManager,
            INotificationService notificationService,
            IConfiguration configuration,
            GigaChat chat)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _projectUsersRepository = projectUsersRepository;
            _columnRepository = columnRepository;
            _unitOfWork = unitOfWork;
            _messageRepository = messageRepository;
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
            var user = await _userRepository.GetUserByIdAsync(taskDtoForCreate.UserId, cancellationToken);
            if (user == null) 
            {
                throw new UserNotFoundException(taskDtoForCreate.UserId);
            }

            //Проверяем приписан ли человек к проекту
            var projectUser = await _projectUsersRepository.GetProjectUser(taskDtoForCreate.UserId, projectId, cancellationToken);
            if (projectUser == null)
            {
                throw new ProjectUsersNotFoundException(taskDtoForCreate.UserId, projectId);
            }

            //Проверяем существует ли вообще колонка, {Id} которой передали
            var column = await _columnRepository.GetColumnByIdAsync(taskDtoForCreate.ColumnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(taskDtoForCreate.ColumnId);
            }

            //Проверяем принадлежит ли колонка, в которую мы пытаемся добавить Task, тому же проекту, в котором мы сейчас находимся(!!)
            var CheckColumnOnProject = await _projectRepository.GetProjectByIdAsync(projectId, cancellationToken);
            var ColumnProject = await _projectRepository.GetProjectByIdAsync(column.ProjectId, cancellationToken);
            if (CheckColumnOnProject != ColumnProject)
            {
                throw new TaskCreatingErrorWithColumnDependency(taskDtoForCreate.ColumnId, projectId);
            }

            var task = _mapper.Map<Domain.Entities.Task>(taskDtoForCreate);
            task.DateEnd = DateTime.SpecifyKind(taskDtoForCreate.DateEnd, DateTimeKind.Utc);
            _taskRepository.Insert(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            string subject = "New Task Created";
            string body = $"Dear {user.Email},\n\nA new task has been created for you: {task.Title}";
            await _notificationService.SendAsync(user.Email, subject, body, cancellationToken);

            return _mapper.Map<TaskDto>(task);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid projectId, Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            var column = await _columnRepository.GetColumnByIdAsync(task.ColumnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(task.ColumnId);
            }

            if (projectId != column.ProjectId)
            {
                throw new TaskCreatingErrorWithColumnDependency(task.ColumnId, projectId);
            }

            _taskRepository.Remove(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<TaskDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var tasks = await _taskRepository.GetAllTasksAsync(cancellationToken);
            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<List<TaskDto>> GetAllTasksForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            var tasks = await _taskRepository.GetAllTasksForUserAsync(userId, cancellationToken);
            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<List<TaskDto>> GetAllTasksForUserInColumnAsync(Guid userId, Guid columnId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            var tasks = await _taskRepository.GetAllTasksForUserInColumnAsync(userId, columnId, cancellationToken);
            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<List<TaskDto>> GetAllTasksInColumnAsync(Guid columnId, CancellationToken cancellationToken = default)
        {
            var tasks = await _taskRepository.GetAllTasksInColumnAsync(columnId, cancellationToken);
            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<TaskDto> GetTaskById(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }
            return _mapper.Map<TaskDto>(task);
        }

        public async Task UpdateAsync(Guid taskId, TaskDtoForUpdate taskDtoForUpdate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(taskDtoForUpdate, cancellationToken);

            var task = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            var user = await _userRepository.GetUserByIdAsync((Guid)taskDtoForUpdate.UserId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException((Guid)taskDtoForUpdate.UserId);
            }

            if (taskDtoForUpdate.ColumnId != Guid.Empty)
            {
                var column = await _columnRepository.GetColumnByIdAsync((Guid)taskDtoForUpdate.ColumnId, cancellationToken);
                if (column == null)
                {
                    throw new ColumnNotFoundException((Guid)taskDtoForUpdate.ColumnId);
                }
            }

            _mapper.Map(taskDtoForUpdate, task);
            if (taskDtoForUpdate.DateEnd != null) { task.DateEnd = DateTime.SpecifyKind((DateTime)taskDtoForUpdate.DateEnd, DateTimeKind.Utc); }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<ChatbotRecommendationsResponseDto> GetResponseRecommendationsByChatBot(Guid projectId, string userMessage, CancellationToken cancellationToken = default)
        {
            if (_chat.Token == null || _chat.Token.ExpiresAt < ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds())
            {
                try
                {
                    await _chat.CreateTokenAsync();
                }
                catch (Exception) { throw new ChatBotUnavailableException(); }
            }

            string taskInfo = 
                "У меня есть название и описание задачи \n" +
                $"{userMessage} \n" +
                "В приоритете нужно указать одно из трёх значений - Low, Medium, High на твоё усмотрение, а в количестве дней только цифру. Мне нужен ответ строго в следующем формате без лишних знаков: \n" +
                "Приоритет: \n " +
                "Количество дней: ";

            string stringResponse;
            try
            {
                var response = await _chat.CompletionsAsync(taskInfo);
                stringResponse = response.choices.LastOrDefault().message.content;
            }
            catch (Exception) { throw new ChatBotUnavailableException(); }

            Match priorityMatch = Regex.Match(stringResponse, @"Приоритет:\s*(Low|Medium|High)", RegexOptions.IgnoreCase);
            Match daysMatch = Regex.Match(stringResponse, @"Количество\s*дней:\s*(\d+)", RegexOptions.IgnoreCase);

            if (!priorityMatch.Success || !daysMatch.Success)
            {
                throw new InvalidOperationException("Невозможно распознать данные из ответа бота.");
            }

            string priorityString = priorityMatch.Groups[1].Value.Trim();
            int numberOfDays = int.Parse(daysMatch.Groups[1].Value.Trim());

            Guid userId = await _taskRepository.GetBestUserEmailForTask(projectId);

            return new ChatbotRecommendationsResponseDto(Enum.Parse<Priority>(priorityString), numberOfDays, userId);
        }

        public async Task<string> GetResponseByChatBot(Guid taskId, string userMessage, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetTaskByIdAsync((Guid)taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            if (_chat.Token == null || _chat.Token.ExpiresAt < ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds())
            {
                try
                {
                    await _chat.CreateTokenAsync();
                }
                catch (Exception) { throw new ChatBotUnavailableException(); }
            }

            if (task.ContextMessages == null || task.ContextMessages.Count == 0)
            {
                var column = await _columnRepository.GetColumnByIdAsync(task.ColumnId, cancellationToken);
                var project = await _projectRepository.GetProjectByIdAsync(column.ProjectId, cancellationToken);

                string taskInfo = $"Название проекта: {project.Name} " +
                    $"\n Название задачи, которую нужно решить: {task.Title} " +
                    $"\n Описание задачи: {task.Description}." +
                    $"\n Дополнительные требования/объяснения: {userMessage}.";
                string stringResponse;
                try
                {
                    var response = await _chat.CompletionsAsync(taskInfo);
                    stringResponse = response.choices.LastOrDefault().message.content;
                }
                catch (Exception) { throw new ChatBotUnavailableException(); }

                task.ContextMessages = new List<string> { taskInfo };
                task.Conversation = new List<Message>
                {
                    new Message { Sender = "user", Text = taskInfo, Timestamp = DateTime.UtcNow },
                    new Message { Sender = "bot", Text = stringResponse, Timestamp = DateTime.UtcNow }
                };
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return stringResponse;
            }

            MessageQuery messageQuery = new MessageQuery(max_tokens: 2048L);
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
            task.Conversation.Add(new Message { Sender = "user", Text = userMessage, Timestamp = DateTime.UtcNow });
            task.Conversation.Add(new Message { Sender = "bot", Text = stringResponseBig, Timestamp = DateTime.UtcNow });
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return stringResponseBig;
        }

        public async Task<List<MessageDto>> GetConversation(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }

            var conversation = await _messageRepository.GetAllMessagesForTaskAsync(taskId, cancellationToken);
            return _mapper.Map<List<MessageDto>>(conversation);
        }

        public async Task<List<string>> GetTaskChatBotContext(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
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
            var task = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
            if (task == null)
            {
                throw new TaskNotFoundException(taskId);
            }
            
            task.ContextMessages = null;
            task.Conversation = null;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
