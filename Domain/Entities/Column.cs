﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Column : IId
    {
        public string Title { get; set; } = string.Empty;

        public Guid ProjectId { get; set; }
    }
}