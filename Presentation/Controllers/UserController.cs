using Contracts.Dtos.UserDtos;
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
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public UserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var users = await _serviceManager.UserService.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            var userDto = await _serviceManager.UserService.GetUserById(userId, cancellationToken);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDtoForCreate userDtoForCreate)
        {
            var userDto = await _serviceManager.UserService.CreateAsync(userDtoForCreate);
            return CreatedAtAction(nameof(GetUserById), new { userId = userDto.Id }, userDto);
        }

        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserDtoForUpdate userDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.UserService.UpdateAsync(userId, userDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            await _serviceManager.UserService.DeleteAsync(userId, cancellationToken);
            return NoContent();
        }
    }
}
