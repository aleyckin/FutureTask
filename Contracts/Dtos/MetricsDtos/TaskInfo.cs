using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.MetricsDtos
{
    public class TaskInfo
    {
        public string Title { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateTime DateCreated { get; set; } 
        public DateTime DateEnd { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
