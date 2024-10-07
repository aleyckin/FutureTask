using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.ProjectUsersDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Services.Attributes;
using System.Runtime.InteropServices;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public ProjectController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
        {
            var projects = await _serviceManager.ProjectService.GetAllAsync(cancellationToken);
            return Ok(projects);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("{projectId:guid}")]
        public async Task<IActionResult> GetProjectById(Guid projectId, CancellationToken cancellationToken)
        {
            var projectDto = await _serviceManager.ProjectService.GetProjectById(projectId, cancellationToken);
            return Ok(projectDto);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDtoForCreate projectDtoForCreate)
        {
            var projectDto = await _serviceManager.ProjectService.CreateAsync(projectDtoForCreate);
            return CreatedAtAction(nameof(GetProjectById), new { projectId = projectDto.Id }, projectDto);
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpPut("{projectId:guid}")]
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] ProjectDtoForUpdate projectDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.ProjectService.UpdateAsync(projectId, projectDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{projectId:guid}")]
        public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellationToken)
        {
            await _serviceManager.ProjectService.DeleteAsync(projectId, cancellationToken);
            return NoContent();
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpGet("projectUsers/users{projectId:guid}")]
        public async Task<IActionResult> GetAllUsersForProject(Guid projectId, CancellationToken cancellationToken)
        {
            var users = await _serviceManager.ProjectUsersService.GetAllUsersByProject(projectId, cancellationToken);

            return Ok(users);
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpPost("projectUsers/addUserToProject")]
        public async Task<IActionResult> AddUserToProject([FromBody] ProjectUsersDto projectUsersDto, CancellationToken cancellationToken)
        {
            await _serviceManager.ProjectUsersService.AddUserToProjectAsync(projectUsersDto, cancellationToken);
            return NoContent();
        }

        [HttpPost("projectUsers/addUserToProjectAsAdmin")]
        public async Task<IActionResult> AddUserToProjectAsAdmin([FromBody] ProjectUsersDto projectUsersDto, CancellationToken cancellationToken)
        {
            await _serviceManager.ProjectUsersService.AddUserToProjectAsync(projectUsersDto, cancellationToken);
            return NoContent();
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpDelete("projectUsers/DeleteUserFromProject")]
        public async Task<IActionResult> DeleteUserFromProject([FromBody] ProjectUsersDto projectUsersDto, CancellationToken cancellationToken)
        {
            await _serviceManager.ProjectUsersService.DeleteUserFromProjectAsync(projectUsersDto, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("projectUsers/DeleteUserFromProjectAsAdmin")]
        public async Task<IActionResult> DeleteUserFromProjectAsAdmin([FromBody] ProjectUsersDto projectUsersDto, CancellationToken cancellationToken)
        {
            await _serviceManager.ProjectUsersService.DeleteUserFromProjectAsync(projectUsersDto, cancellationToken);
            return NoContent();
        }
    }
}
