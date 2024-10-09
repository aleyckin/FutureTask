using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.ColumnExceptions
{
    public sealed class ColumnCreatingErrorWithProjectDependency : DestroyingLogicalDependencyException
    {
         public ColumnCreatingErrorWithProjectDependency(Guid projectIdController, Guid projectId) : base($"The identifier of project is difference: {projectIdController} => {projectId}.") { }
    }
}
