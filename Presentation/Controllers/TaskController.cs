using Contracts.Dtos.TaskDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetTasks(CancellationToken cancellationToken)
        {
            return await _taskService.GetAllAsync(cancellationToken);
        }

        [HttpGet("userTasks")]
        public async Task<ActionResult<List<TaskDto>>> GetUserTasks(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = new Guid(userIdClaim);
            return await _taskService.GetAllTasksForUserAsync(userId, cancellationToken);
        }

        [HttpGet("allTasksInColumn/{columnId:guid}")]
        public async Task<ActionResult<List<TaskDto>>> GetAllTasksInColumn(Guid columnId, CancellationToken cancellationToken)
        {
            return await _taskService.GetAllTasksInColumnAsync(columnId, cancellationToken);
        }

        [HttpGet("userTasksInColumn/{columnId:guid}")]
        public async Task<ActionResult<List<TaskDto>>> GetAllTasksForUserInColumn(Guid columnId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = new Guid(userIdClaim);
            return await _taskService.GetAllTasksForUserInColumnAsync(userId, columnId, cancellationToken);
        }

        [HttpGet("{taskId:guid}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(Guid taskId, CancellationToken cancellationToken)
        {
            return await _taskService.GetTaskById(taskId, cancellationToken);
        }

        [HttpPost("{projectId:guid}")]
        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] TaskDtoForCreate taskDtoForCreate, CancellationToken cancellationToken)
        {
            var taskDto = await _taskService.CreateAsync(projectId, taskDtoForCreate, cancellationToken);
            return CreatedAtAction(nameof(GetTaskById), new { taskId = taskDto.Id }, taskDto);
        }

        [HttpPut("{taskId:guid}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskDtoForUpdate taskDtoForUpdate, CancellationToken cancellationToken)
        {
            await _taskService.UpdateAsync(taskId, taskDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{taskId:guid}/{projectId:guid}")]
        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        public async Task<IActionResult> DeleteTask(Guid taskId, Guid projectId, CancellationToken cancellationToken)
        {
            await _taskService.DeleteAsync(projectId, taskId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{taskId:guid}/chatBot")]
        public async Task<IActionResult> GetChatBotResponse(Guid taskId, [FromBody] string userMessage, CancellationToken cancellationToken)
        {
            var response = await _taskService.GetResponseByChatBot(taskId, userMessage, cancellationToken);
            return Ok(new { responseMessage = response });
        }

        [HttpGet("{taskId:guid}/chatBot")]
        public async Task<IActionResult> GetTaskChatBotContext(Guid taskId, CancellationToken cancellationToken)
        {
            var response = await _taskService.GetTaskChatBotContext(taskId, cancellationToken);
            return Ok(new { responseMessage = response });
        }

        [HttpGet("{taskId:guid}/chatBot/conversation")]
        public async Task<IActionResult> GetConversation(Guid taskId, CancellationToken cancellationToken)
        {
            var response = await _taskService.GetConversation(taskId, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("{taskId:guid}/chatBot")]
        public async Task<IActionResult> DeleteTaskChatBotContext(Guid taskId, CancellationToken cancellationToken)
        {
            await _taskService.DeleteTaskChatBotContext(taskId, cancellationToken);
            return NoContent();
        }
    }
}
