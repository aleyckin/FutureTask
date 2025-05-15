using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.MetricsDtos
{
    public class ProjectInfo
    {
        public string Title { get; set; } = string.Empty;
        public RoleOnProject RoleOnProject { get; set; } = RoleOnProject.DefaultWorker;
        public int CountOfTasks { get; set; } = 0;
        public int CountOfCompetedTasks { get; set; } = 0;
        public List<TaskInfo> TaskInfos = new();
    }
}
