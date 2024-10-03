using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.TaskDtos
{
    public class TaskDtoForUpdate
    {
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public Priority? Priority { get; set; }
        public Status? Status { get; set; }
        public DateTime? DateEnd { get; set; }
        public Guid? UserId { get; set; }
    }
}
