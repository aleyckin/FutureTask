using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Guid AdministratorId { get; set; }

        public List<User> Users { get; set; } = new List<User>();
        public List<ProjectUsers> ProjectUsers { get; set; } = new List<ProjectUsers>();
    }
}
