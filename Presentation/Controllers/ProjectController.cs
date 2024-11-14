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
        private readonly IProjectService _projectService;
        private readonly IProjectUsersService _projectUsersService;

        public ProjectController(IProjectService projectService, IProjectUsersService projectUsersService)
        {
            _projectService = projectService;
            _projectUsersService = projectUsersService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
        {
            var projects = await _projectService.GetAllAsync(cancellationToken);
            return Ok(projects);
        }

        [HttpGet("{projectId:guid}")]
        public async Task<IActionResult> GetProjectById(Guid projectId, CancellationToken cancellationToken)
        {
            var projectDto = await _projectService.GetProjectById(projectId, cancellationToken);
            return Ok(projectDto);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDtoForCreate projectDtoForCreate)
        {
            var projectDto = await _projectService.CreateAsync(projectDtoForCreate);
            return CreatedAtAction(nameof(GetProjectById), new { projectId = projectDto.Id }, projectDto);
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpPut("{projectId:guid}")]
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] ProjectDtoForUpdate projectDtoForUpdate, CancellationToken cancellationToken)
        {
            await _projectService.UpdateAsync(projectId, projectDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{projectId:guid}")]
        public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellationToken)
        {
            await _projectService.DeleteAsync(projectId, cancellationToken);
            return NoContent();
        }

        [HttpGet("projectUsers/{projectId:guid}/users")]
        public async Task<IActionResult> GetAllUsersForProject(Guid projectId, CancellationToken cancellationToken)
        {
            var users = await _projectUsersService.GetAllUsersByProject(projectId, cancellationToken);

            return Ok(users);
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpPost("projectUsers/addUserToProject:{projectId:guid}")]
        public async Task<IActionResult> AddUserToProject(Guid projectId,[FromBody] ProjectUsersDto projectUsersDto, CancellationToken cancellationToken)
        {
            await _projectUsersService.AddUserToProjectAsync(projectUsersDto, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("projectUsers/addUserToProjectAsAdmin")]
        public async Task<IActionResult> AddUserToProjectAsAdmin([FromBody] ProjectUsersDto projectUsersDto, CancellationToken cancellationToken)
        {
            await _projectUsersService.AddUserToProjectAsync(projectUsersDto, cancellationToken);
            return NoContent();
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpDelete("projectUsers/deleteUserFromProject:{projectId:guid}")]
        public async Task<IActionResult> DeleteUserFromProject(Guid userId, Guid projectId, CancellationToken cancellationToken)
        {
            await _projectUsersService.DeleteUserFromProjectAsync(userId, projectId, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("projectUsers/deleteUserFromProjectAsAdmin/{userId:guid}/{projectId:guid}")]
        public async Task<IActionResult> DeleteUserFromProjectAsAdmin(Guid userId, Guid projectId, CancellationToken cancellationToken)
        {
            await _projectUsersService.DeleteUserFromProjectAsync(userId, projectId, cancellationToken);
            return NoContent();
        }
    }
}
