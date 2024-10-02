using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Task : IId
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEnd { get; set; }
        public Guid UserId { get; set; }
        public Guid ColumnId { get; set; }
    }
}
