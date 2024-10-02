using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.ProjectDtos
{
    public class ProjectDto : IId
    {
        public string Name { get; set; } = string.Empty; 
        public Guid AdministratorId { get; set; }
    }
}
