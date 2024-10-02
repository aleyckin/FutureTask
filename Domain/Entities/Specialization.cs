using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Specialization : IId
    {
        public string Name { get; set; } = string.Empty;

        public List<User> Users { get; set; } = new List<User>();
    }
}
