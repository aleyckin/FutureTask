using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.ColumnDtos
{
    public class ColumnDtoForUpdate
    {
        public string Title { get; set; } = string.Empty;
        public List<Domain.Entities.Task> Tasks { get; set; } = new List<Domain.Entities.Task>();
    }
}
