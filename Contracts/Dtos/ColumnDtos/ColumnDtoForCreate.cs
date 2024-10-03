using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.ColumnDtos
{
    public class ColumnDtoForCreate
    {
        public string Title { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
    }
}
