using Contracts.Dtos.ColumnDtos;
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
    [Authorize]
    [ApiController]
    [Route("api/columns")]
    public class ColumnController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public ColumnController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetColumns(CancellationToken cancellationToken)
        {
            var columns = await _serviceManager.ColumnService.GetAllAsync(cancellationToken);
            return Ok(columns);
        }

        [Authorize]
        [HttpGet("ForProject:{projectId:guid}")]
        public async Task<IActionResult> GetColumnsForProject(Guid projectId, CancellationToken cancellationToken)
        {
            var columns = await _serviceManager.ColumnService.GetAllColumnsForProjectAsync(projectId, cancellationToken);
            return Ok(columns);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("{columnId:guid}")]
        public async Task<IActionResult> GetColumnById(Guid columnId, CancellationToken cancellationToken)
        {
            var columnDto = await _serviceManager.ColumnService.GetColumnById(columnId, cancellationToken);
            return Ok(columnDto);
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpPost("{projectId:guid}")]
        public async Task<IActionResult> CreateColumn(Guid projectId, [FromBody] ColumnDtoForCreate columnDtoForCreate)
        {
            var columnDto = await _serviceManager.ColumnService.CreateAsync(projectId, columnDtoForCreate);
            return CreatedAtAction(nameof(GetColumnById), new { columnId = columnDto.Id }, columnDto);
        }

        [Authorize]
        [HttpPut("{columnId:guid}")]
        public async Task<IActionResult> UpdateColumn(Guid columnId, [FromBody] ColumnDtoForUpdate columnDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.ColumnService.UpdateAsync(columnId, columnDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpDelete("{projectId:guid}&&{columnId:guid}")]
        public async Task<IActionResult> DeleteColumn(Guid projectId, Guid columnId, CancellationToken cancellationToken)
        {
            await _serviceManager.ColumnService.DeleteAsync(projectId, columnId, cancellationToken);
            return NoContent();
        }
    }
}
