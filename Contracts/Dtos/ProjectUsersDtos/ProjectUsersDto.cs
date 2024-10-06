using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.ProjectUsersDtos
{
    public record ProjectUsersDto(Guid UserId, Guid ProjectId, RoleOnProject? RoleOnProject);
    public record ProjectUsersDtoForListUsers(string UserEmail, RoleOnProject RoleOnProject);
    public record ProjectUsersDtoForListProjects(string ProjectName, RoleOnProject RoleOnProject);
}
