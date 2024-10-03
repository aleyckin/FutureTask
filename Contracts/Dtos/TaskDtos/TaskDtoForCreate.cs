using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.TaskDtos
{
    public class TaskDtoForCreate
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public Status Status { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateEnd { get; set; }
        public Guid UserId { get; set; }
        public Guid Column { get; set; }
    }
}
