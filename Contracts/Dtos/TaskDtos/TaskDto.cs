using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.TaskDtos
{
    public record TaskDto(Guid Id, string Title, string Description, Priority Priority, Status Status, DateTime DateCreated, DateTime DateEnd, Guid UserId, Guid ColumnId) { }
    public record TaskDtoForCreate(string Title, string Description, Priority Priority, DateTime DateEnd, Guid UserId, Guid ColumnId) { }
    public record TaskDtoForUpdate(string? Title, string? Description, Priority? Priority, Status? Status, DateTime? DateEnd, Guid? UserId, Guid? ColumnId) { }
}
