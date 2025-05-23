using Contracts.Dtos.ProjectUsersDtos;
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
        private readonly IUserService _userService;
        private readonly IProjectUsersService _projectUsersService;

        public UserController(IUserService userService, IProjectUsersService projectUsersService)
        {
            _userService = userService;
            _projectUsersService = projectUsersService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers(CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsync(cancellationToken);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            return await _userService.GetUserById(userId, cancellationToken);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDtoForCreate userDtoForCreate)
        {
            var userDto = await _userService.CreateAsync(userDtoForCreate);
            return CreatedAtAction(nameof(GetUserById), new { userId = userDto.Id }, userDto);
        }

        [Authorize]
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserDtoForUpdate userDtoForUpdate, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(userId, userDtoForUpdate, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            await _userService.DeleteAsync(userId, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("projectUsers/projects")]
        public async Task<ActionResult<List<ProjectUsersDtoForListProjects>>> GetAllProjectsAsAdmin(CancellationToken cancellationToken)
        {
            return await _projectUsersService.GetAllProjectsAsAdmin(cancellationToken);
        }

        [Authorize]
        [HttpGet("projectUsers/projectsForRegularUser")]
        public async Task<ActionResult<List<ProjectUsersDtoForListProjects>>> GetAllProjectsForUser(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = new Guid(userIdClaim);
            return await _projectUsersService.GetAllProjectsByUser(userId, cancellationToken);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var userDto = await _userService.ValidateUserCredentials(loginDto.Email, loginDto.Password, cancellationToken);
            if (userDto == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _userService.GenerateJwtToken(userDto);

            return Ok(new { token, user = userDto });
        }
    }
}
