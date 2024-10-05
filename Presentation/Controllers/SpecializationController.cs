using Contracts.Dtos.SpecializationDtos;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/specializations")]
    public class SpecializationController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public SpecializationController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecializations(CancellationToken cancellationToken)
        {
            var specializations = await _serviceManager.SpecializationService.GetAllAsync(cancellationToken);
            return Ok(specializations);
        }

        [HttpGet("{specializationId:guid}")]
        public async Task<IActionResult> GetSpecializationById(Guid specializationId, CancellationToken cancellationToken)
        {
            var specializationDto = await _serviceManager.SpecializationService.GetSpecializationById(specializationId, cancellationToken);
            return Ok(specializationDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialization([FromBody] SpecializationDtoForCreate specializationDtoForCreate)
        {
            var specializationDto = await _serviceManager.SpecializationService.CreateAsync(specializationDtoForCreate);
            return CreatedAtAction(nameof(GetSpecializationById), new { specializationId = specializationDto.Id }, specializationDto);
        }

        [HttpPut("{specializationId:guid}")]
        public async Task<IActionResult> UpdateSpecialization(Guid specializationId, [FromBody] SpecializationDtoForUpdate specializationDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.SpecializationService.UpdateAsync(specializationId, specializationDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{specializationId:guid}")]
        public async Task<IActionResult> DeleteSpecialization(Guid specializationId, CancellationToken cancellationToken)
        {
            await _serviceManager.SpecializationService.DeleteAsync(specializationId, cancellationToken);
            return NoContent();
        }
    }
}
