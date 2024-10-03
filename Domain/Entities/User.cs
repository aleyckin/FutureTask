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
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public Guid SpecializationId { get; set; }
        public Specialization Specialization { get; set; } = null!;
        
        public List<Domain.Entities.Task> Tasks { get; set; } = new List<Domain.Entities.Task> { };

        public List<ProjectUsers> ProjectUsers { get; set; } = new List<ProjectUsers>();
    }
}
