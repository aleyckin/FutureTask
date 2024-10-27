using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Helpers;

namespace Domain.Entities
{
    public class Project : IId
    {
        public string Name { get; set; } = string.Empty;

        public List<Column> Columns { get; set; } = new List<Column>();

        public List<ProjectUsers> ProjectUsers { get; set; } = new List<ProjectUsers>();
    }
}
