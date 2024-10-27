using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Helpers;

namespace Domain.Entities
{
    public class Column : IId
    {
        public string Title { get; set; } = string.Empty;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public List<Domain.Entities.Task> Tasks { get; set; } = new List<Domain.Entities.Task>();
    }
}
