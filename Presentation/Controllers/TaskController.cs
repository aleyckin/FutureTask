﻿using Contracts.Dtos.TaskDtos;
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
        private readonly IServiceManager _serviceManager;
        public TaskController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
        {
            var tasks = await _serviceManager.TaskService.GetAllAsync(cancellationToken);
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("userTasks")]
        public async Task<IActionResult> GetUserTasks(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = new Guid(userIdClaim);
            var tasks = await _serviceManager.TaskService.GetAllTasksForUserAsync(userId, cancellationToken);
            return Ok(tasks);
        }

        [HttpGet("{taskId:guid}")]
        public async Task<IActionResult> GetTaskById(Guid taskId, CancellationToken cancellationToken)
        {
            var taskDto = await _serviceManager.TaskService.GetTaskById(taskId, cancellationToken);
            return Ok(taskDto);
        }

        [HttpPost("{projectId:guid}")]
        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] TaskDtoForCreate taskDtoForCreate, CancellationToken cancellationToken)
        {
            var taskDto = await _serviceManager.TaskService.CreateAsync(projectId, taskDtoForCreate, cancellationToken);
            return CreatedAtAction(nameof(GetTaskById), new { taskId = taskDto.Id }, taskDto);
        }

        [HttpPut("{taskId:guid}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskDtoForUpdate taskDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.TaskService.UpdateAsync(taskId, taskDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{taskId:guid}&&{projectId:guid}")]
        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        public async Task<IActionResult> DeleteTask(Guid taskId, Guid projectId, CancellationToken cancellationToken)
        {
            await _serviceManager.TaskService.DeleteAsync(projectId, taskId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{taskId:guid}/chatBot")]
        public async Task<IActionResult> GetChatBotResponse(Guid taskId, string userMessage, CancellationToken cancellationToken)
        {
            string response = await _serviceManager.TaskService.GetResponseByChatBot(taskId, userMessage, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{taskId:guid}/chatBot")]
        public async Task<IActionResult> GetTaskChatBotContext(Guid taskId, CancellationToken cancellationToken)
        {
            List<string> response = await _serviceManager.TaskService.GetTaskChatBotContext(taskId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{taskId:guid}/chatBot/conversation")]
        public async Task<IActionResult> GetConversation(Guid taskId, CancellationToken cancellationToken)
        {
            List<string> response = await _serviceManager.TaskService.GetConversation(taskId, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("{taskId:guid}/chatBot")]
        public async Task<IActionResult> DeleteTaskChatBotContext(Guid taskId, CancellationToken cancellationToken)
        {
            await _serviceManager.TaskService.DeleteTaskChatBotContext(taskId, cancellationToken);
            return NoContent();
        }
    }
}
