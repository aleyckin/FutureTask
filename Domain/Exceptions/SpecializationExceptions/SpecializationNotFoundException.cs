using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.SpecializationExceptions
{
    public sealed class SpecializationNotFoundException : NotFoundException
    {
        public SpecializationNotFoundException(Guid specializatioId) : base($"The Specialization with the identifier {specializatioId} not found.") { }
    }
}
