using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.UserDtos
{
    public class UserDto : IId
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Guid SpecializationId { get; set; }
        public List<ProjectUsers> ProjectUsers { get; set; } = new List<ProjectUsers>();
    }
}
