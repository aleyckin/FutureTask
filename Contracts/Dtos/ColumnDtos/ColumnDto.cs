using Contracts.Dtos.TaskDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.ColumnDtos
{
    public record ColumnDto(Guid Id, string Title, Guid ProjectId, List<TaskDto> Tasks) { }
    public record ColumnDtoForCreate(string Title, Guid ProjectId) { }
    public record ColumnDtoForUpdate(string? Title, List<TaskDto>? Tasks) { }
}
