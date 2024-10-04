using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.ProjectExceptions
{
    public sealed class ProjectNotFoundException : NotFoundException
    {
        public ProjectNotFoundException(Guid projectId) : base($"The Project with the identifier {projectId} not found.") { }
    }
}
