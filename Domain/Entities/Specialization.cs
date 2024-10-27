using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities.Helpers;

namespace Domain.Entities
{
    public class Specialization : IId
    {
        public string Name { get; set; } = string.Empty;
    }
}
