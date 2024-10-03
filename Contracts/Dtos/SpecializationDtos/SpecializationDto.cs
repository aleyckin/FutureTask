using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.SpecializationDtos
{
    public class SpecializationDto : IId
    {
        public string Name { get; set; } = string.Empty;

        public List<User> Users { get; set; } = new List<User>();
    }
}
