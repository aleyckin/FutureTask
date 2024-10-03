using Contracts.Dtos.TaskDtos;
using Contracts.Dtos.UserDtos;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
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
        private readonly ServiceManager _serviceManager;
        public TaskController(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
        {
            var tasks = await _serviceManager.TaskService.GetAllAsync(cancellationToken);
            return Ok(tasks);
        }

        [HttpGet("{taskId:guid}")]
        public async Task<IActionResult> GetTaskById(Guid taskId, CancellationToken cancellationToken)
        {
            var taskDto = await _serviceManager.TaskService.GetTaskById(taskId, cancellationToken);
            return Ok(taskDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskDtoForCreate taskDtoForCreate)
        {
            var taskDto = await _serviceManager.TaskService.CreateAsync(taskDtoForCreate);
            return CreatedAtAction(nameof(GetTaskById), new { taskId = taskDto.Id }, taskDto);
        }

        [HttpPut("{taskId:guid}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskDtoForUpdate taskDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.TaskService.UpdateAsync(taskId, taskDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{taskId:guid}")]
        public async Task<IActionResult> DeleteTask(Guid taskId, CancellationToken cancellationToken)
        {
            await _serviceManager.TaskService.DeleteAsync(taskId, cancellationToken);
            return NoContent();
        }
    }
}
