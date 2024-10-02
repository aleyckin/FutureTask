using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : IId
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public Guid SpecializationId { get; set; }

        public List<Project> Projects { get; set; } = new List<Project>();
        public List<ProjectUsers> ProjectUsers { get; set; } = new List<ProjectUsers>();
    }
}
