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
        private readonly IColumnService _columnService;
        public ColumnController(IColumnService columnService)
        {
            _columnService = columnService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<List<ColumnDto>>> GetColumns(CancellationToken cancellationToken)
        {
            return await _columnService.GetAllAsync(cancellationToken);
        }

        [HttpGet("forProject/{projectId:guid}")]
        public async Task<ActionResult<List<ColumnDto>>> GetColumnsForProject(Guid projectId, CancellationToken cancellationToken)
        {
            return await _columnService.GetAllColumnsForProjectAsync(projectId, cancellationToken);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("{columnId:guid}")]
        public async Task<ActionResult<ColumnDto>> GetColumnById(Guid columnId, CancellationToken cancellationToken)
        {
            return await _columnService.GetColumnById(columnId, cancellationToken);
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpPost("{projectId:guid}")]
        public async Task<IActionResult> CreateColumn(Guid projectId, [FromBody] ColumnDtoForCreate columnDtoForCreate)
        {
            var columnDto = await _columnService.CreateAsync(projectId, columnDtoForCreate);
            return CreatedAtAction(nameof(GetColumnById), new { columnId = columnDto.Id }, columnDto);
        }

        [Authorize]
        [HttpPut("{columnId:guid}")]
        public async Task<IActionResult> UpdateColumn(Guid columnId, [FromBody] ColumnDtoForUpdate columnDtoForUpdate, CancellationToken cancellationToken)
        {
            await _columnService.UpdateAsync(columnId, columnDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [ProjectRoleAuthorize(Domain.Entities.Enums.RoleOnProject.TeamLead)]
        [HttpDelete("{projectId:guid}/{columnId:guid}")]
        public async Task<IActionResult> DeleteColumn(Guid projectId, Guid columnId, CancellationToken cancellationToken)
        {
            await _columnService.DeleteAsync(projectId, columnId, cancellationToken);
            return NoContent();
        }
    }
}
