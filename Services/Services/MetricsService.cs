using Contracts.Dtos.MetricsDtos;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using QuestPDF.Fluent;
using Services.Abstractions;
using Services.PdfBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class MetricsService : IMetricsService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectUsersRepository _projectUsersRepository;
        private readonly IColumnRepository _columnRepository;

        public MetricsService(
            IProjectRepository projectRepository, ITaskRepository taskRepository,
            IProjectUsersRepository projectUsersRepository, IColumnRepository columnRepository)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _projectUsersRepository = projectUsersRepository;
            _columnRepository = columnRepository;
        }

        public async Task<byte[]> GetReportForProject(Guid projectId)
        {
            var project = await _projectRepository.GetProjectByIdAsync(projectId, default);
            var projUsers = await _projectUsersRepository.GetAllUsersByProject(projectId);

            var doneColumn = project.Columns.FirstOrDefault(c => c.Title == "Готово к тестированию");
            var usersInfo = projUsers.Select(u => new UserInfo
            {
                UserId = u.UserId,
                Email = u.User.Email,
                Role = u.RoleOnProject
            }).ToList();

            int totalTasks = 0, doneTasks = 0;

            foreach (var column in project.Columns)
            {
                var tasks = await _taskRepository.GetAllTasksInColumnAsync(column.Id);
                totalTasks += tasks.Count;

                if (doneColumn != null && column.Id == doneColumn.Id)
                    doneTasks = tasks.Count;

                foreach (var user in usersInfo)
                {
                    var userTasks = tasks.Count(t => t.UserId == user.UserId);
                    user.CountOfTasks += userTasks;
                    user.CompletedTasks += (doneColumn != null && column.Id == doneColumn.Id)
                                              ? userTasks : 0;
                }
            }

            var doc = new ProjectReportDocument(project, totalTasks, doneTasks, usersInfo);
            return doc.GeneratePdf();
        }


        public async System.Threading.Tasks.Task<byte[]> GetReportForUser(Guid UserId)
        {
            List<ProjectUsers> projects = await _projectUsersRepository.GetAllProjectsByUser(UserId);
            List<ProjectInfo> projectInfos = new();
            List<TaskInfo> taskInfos = new();
            int CompletedTasks = 0;

            foreach (ProjectUsers projectUser in projects)
            {
                int TaskCount = 0;
                taskInfos = new();
                List<Column> columns = await _columnRepository.GetAllColumnsForProjectAsync(projectUser.ProjectId);
                foreach (Column column in columns)
                {
                    List<Domain.Entities.Task> tasks = await _taskRepository.GetAllTasksForUserInColumnAsync(UserId, column.Id);
                    foreach (Domain.Entities.Task task in tasks)
                    {
                        taskInfos.Add(new TaskInfo
                        {
                            Title = task.Title,
                            Priority = task.Priority,
                            DateCreated = task.DateCreated,
                            DateEnd = task.DateCreated,
                            Status = column.Title
                        });
                    }
                    TaskCount += tasks.Count;
                    if (column.Title == "Готово к тестированию")
                    {
                        CompletedTasks = tasks.Count;
                    }
                }

                projectInfos.Add(new ProjectInfo
                {
                    Title = projectUser.Project.Name,
                    RoleOnProject = projectUser.RoleOnProject,
                    CountOfTasks = TaskCount,
                    CountOfCompetedTasks = CompletedTasks,
                    TaskInfos = taskInfos
                });
            }

            var doc = new UserReportDocument(projectInfos);
            return doc.GeneratePdf();
        }
    }
}
