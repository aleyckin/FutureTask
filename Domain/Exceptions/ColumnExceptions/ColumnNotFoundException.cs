using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.ColumnException
{
    public sealed class ColumnNotFoundException : NotFoundException
    {
        public ColumnNotFoundException(Guid columnId) : base($"The Column with the identifier {columnId} not found.") { }
    }
}
