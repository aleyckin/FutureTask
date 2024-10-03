using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.ColumnDtos
{
    public class ColumnDto : IId
    {
        public string Title { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public List<Domain.Entities.Task> Tasks { get; set; } = new List<Domain.Entities.Task>();
    }
}
