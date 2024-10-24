using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.UserDtos
{
    public record UserDto(Guid Id, string Email, string Password, UserRole UserRole, string SpecializationName) { }
    public record UserDtoForCreate(string Email, string Password, Guid SpecializationId, UserRole UserRole = UserRole.RegularUser) { }
    public record UserDtoForUpdate(string? Email, string? Password, Guid? SpecializationId) { }
    public record LoginDto(string Email, string Password) { }
    public record UserWithRoleOnProjectDto(Guid UserId, string UserName, RoleOnProject RoleOnProject) { }
}
