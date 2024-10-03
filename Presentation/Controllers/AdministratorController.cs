using Contracts.Dtos.AdministratorDtos;
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
    [Route("api/administrators")]
    public class AdministratorController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AdministratorController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAdministrators(CancellationToken cancellationToken)
        {
            var administrators = await _serviceManager.AdministratorService.GetAllAsync(cancellationToken);
            return Ok(administrators);
        }

        [HttpGet("{administratorId:guid}")]
        public async Task<IActionResult> GetAdministratorById(Guid administratorId, CancellationToken cancellationToken)
        {
            var administratorDto = await _serviceManager.AdministratorService.GetAdministratorById(administratorId, cancellationToken);
            return Ok(administratorDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdministrator([FromBody] AdministratorDtoForCreate administratorDtoForCreate)
        {
            var administratorDto = await _serviceManager.AdministratorService.CreateAsync(administratorDtoForCreate);
            return CreatedAtAction(nameof(GetAdministratorById), new { administratorId = administratorDto.Id }, administratorDto);
        }

        [HttpPut("{administratorId:guid}")]
        public async Task<IActionResult> UpdateAdministrator(Guid administratorId, [FromBody] AdministratorDtoForUpdate administratorDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.AdministratorService.UpdateAsync(administratorId, administratorDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{administratorId:guid}")]
        public async Task<IActionResult> DeleteAdministrator(Guid administratorId, CancellationToken cancellationToken)
        {
            await _serviceManager.AdministratorService.DeleteAsync(administratorId, cancellationToken);
            return NoContent();
        }

    }
}
