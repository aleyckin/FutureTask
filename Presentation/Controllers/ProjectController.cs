using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.UserDtos;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly ServiceManager _serviceManager;
        public ProjectController(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
        {
            var projects = await _serviceManager.ProjectService.GetAllAsync(cancellationToken);
            return Ok(projects);
        }

        [HttpGet("{projectId:guid}")]
        public async Task<IActionResult> GetProjectById(Guid userId, CancellationToken cancellationToken)
        {
            var projectDto = await _serviceManager.ProjectService.GetProjectById(userId, cancellationToken);
            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDtoForCreate projectDtoForCreate)
        {
            var projectDto = await _serviceManager.ProjectService.CreateAsync(projectDtoForCreate);
            return CreatedAtAction(nameof(GetProjectById), new { projectId = projectDto.Id }, projectDto);
        }

        [HttpPut("{projectId:guid}")]
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] ProjectDtoForUpdate projectDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.ProjectService.UpdateAsync(projectId, projectDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{projectId:guid}")]
        public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellationToken)
        {
            await _serviceManager.ProjectService.DeleteAsync(projectId, cancellationToken);
            return NoContent();
        }
    }
}
