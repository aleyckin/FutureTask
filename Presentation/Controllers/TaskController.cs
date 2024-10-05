using Contracts.Dtos.TaskDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public TaskController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
        {
            var tasks = await _serviceManager.TaskService.GetAllAsync(cancellationToken);
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("{taskId:guid}")]
        public async Task<IActionResult> GetTaskById(Guid taskId, CancellationToken cancellationToken)
        {
            var taskDto = await _serviceManager.TaskService.GetTaskById(taskId, cancellationToken);
            return Ok(taskDto);
        }

        [Authorize]
        [HttpPost("{projectId:guid}")]
        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] TaskDtoForCreate taskDtoForCreate)
        {
            var taskDto = await _serviceManager.TaskService.CreateAsync(taskDtoForCreate);
            return CreatedAtAction(nameof(GetTaskById), new { taskId = taskDto.Id }, taskDto);
        }

        [Authorize]
        [HttpPut("{taskId:guid}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskDtoForUpdate taskDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.TaskService.UpdateAsync(taskId, taskDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{taskId:guid}")]
        public async Task<IActionResult> DeleteTask(Guid taskId, CancellationToken cancellationToken)
        {
            await _serviceManager.TaskService.DeleteAsync(taskId, cancellationToken);
            return NoContent();
        }
    }
}
