using Contracts.Dtos.ProjectDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.AdministratorDtos
{
    public class AdministratorDto : IId
    {
        public string Email { get; set; } = string.Empty;
        public List<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    }
}
