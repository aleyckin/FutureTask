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
        public Status Status { get; set; } = Status.OnQueue;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateTime DateCreated { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        public DateTime DateEnd { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid ColumnId { get; set; }
        public Column? Column { get; set; }
        public List<string>? ContextMessages { get; set; }
        public List<string>? Conversation { get; set; }
    }
}
