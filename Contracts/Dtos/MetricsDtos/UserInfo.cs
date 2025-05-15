using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.MetricsDtos
{
    public class UserInfo() 
    { 
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public RoleOnProject Role { get; set; } = RoleOnProject.DefaultWorker;
        public int CountOfTasks { get; set; } = 0;
        public int CompletedTasks { get; set; } = 0;
    }
}
