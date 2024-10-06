using Contracts.Dtos.UserDtos;
using Domain.Entities.Enums;
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
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public UserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var users = await _serviceManager.UserService.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            var userDto = await _serviceManager.UserService.GetUserById(userId, cancellationToken);
            return Ok(userDto);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDtoForCreate userDtoForCreate)
        {
            var userDto = await _serviceManager.UserService.CreateAsync(userDtoForCreate);
            return CreatedAtAction(nameof(GetUserById), new { userId = userDto.Id }, userDto);
        }

        [Authorize]
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserDtoForUpdate userDtoForUpdate, CancellationToken cancellationToken)
        {
            await _serviceManager.UserService.UpdateAsync(userId, userDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            await _serviceManager.UserService.DeleteAsync(userId, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("projectUsers/projectsFor:{userId:guid}")]
        public async Task<IActionResult> GetAllProjectsForUser(Guid userId, CancellationToken cancellationToken)
        {
            var projects = await _serviceManager.ProjectUsersService.GetAllProjectsByUser(userId, cancellationToken);

            return Ok(projects);
        }

        [Authorize]
        [HttpGet("projectUsers/projectsForRegularUser")]
        public async Task<IActionResult> GetAllProjectsForUser(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = new Guid(userIdClaim);
            var projects = await _serviceManager.ProjectUsersService.GetAllProjectsByUser(userId, cancellationToken);

            return Ok(projects);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var userDto = await _serviceManager.UserService.ValidateUserCredentials(loginDto.Email, loginDto.Password, cancellationToken);
            if (userDto == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _serviceManager.UserService.GenerateJwtToken(userDto);

            return Ok(token);
        }
    }
}
