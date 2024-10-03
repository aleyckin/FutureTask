using Contracts.Dtos.ColumnDtos;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/columns")]
    public class ColumnController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public ColumnController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetColumns(CancellationToken cancellationToken)
        {
            var columns = await _serviceManager.ColumnService.GetAllAsync(cancellationToken);
            return Ok(columns);
        }

        [HttpGet("{columnId:guid}")]
        public async Task<IActionResult> GetColumnById(Guid columnId, CancellationToken cancellationToken)
        {
            var columnDto = await _serviceManager.ColumnService.GetColumnById(columnId, cancellationToken);
            return Ok(columnDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateColumn([FromBody] ColumnDtoForCreate columnDtoForCreate)
        {
            var columnDto = await _serviceManager.ColumnService.CreateAsync(columnDtoForCreate);
            return CreatedAtAction(nameof(GetColumnById), new { columnId = columnDto.Id }, columnDto);
        }

        [HttpPut("{columnId:guid}")]
        public async Task<IActionResult> UpdateColumn(Guid columnId, [FromBody] ColumnDtoForUpdate columnDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.ColumnService.UpdateAsync(columnId, columnDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{columnId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            await _serviceManager.UserService.DeleteAsync(userId, cancellationToken);
            return NoContent();
        }
    }
}
