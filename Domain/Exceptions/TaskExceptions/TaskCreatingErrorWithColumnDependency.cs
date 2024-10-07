using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.TaskExceptions
{
    public class TaskCreatingErrorWithColumnDependency : DestroyingLogicalDependencyException
    {
        public TaskCreatingErrorWithColumnDependency(Guid columnId, Guid projectId) : base($"The column with identifier {columnId} not belongs to project with identifier {projectId}.") { }
    }
}
