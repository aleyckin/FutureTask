using Contracts.Dtos.ColumnDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.ProjectDtos
{
    public record ProjectDto(Guid Id, string Name, List<ColumnDto> Columns) { }
    public record ProjectDtoForCreate(string Name) { }
    public record ProjectDtoForUpdate(string? Name) { }
}
