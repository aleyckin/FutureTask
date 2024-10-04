using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.TaskExceptions
{
    public sealed class TaskNotFoundException : NotFoundException
    {
        public TaskNotFoundException(Guid taskId) : base($"The Task with the identifier {taskId} not found.") { }
    }
}
