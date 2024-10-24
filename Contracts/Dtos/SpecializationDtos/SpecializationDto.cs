using Contracts.Dtos.UserDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.SpecializationDtos
{
    public record SpecializationDto(Guid Id, string Name) { }
    public record SpecializationDtoForCreate(string Name) { }
    public record SpecializationDtoForUpdate(string? Name) { }
}
