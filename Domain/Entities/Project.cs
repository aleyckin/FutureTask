﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project : IId
    {
        public string Name { get; set; } = string.Empty;

        public Guid AdministratorId { get; set; }

        public List<Column> Columns { get; set; } = new List<Column>();

        public List<User> Users { get; set; } = new List<User>();
        public List<ProjectUsers> ProjectUsers { get; set; } = new List<ProjectUsers>();
    }
}
