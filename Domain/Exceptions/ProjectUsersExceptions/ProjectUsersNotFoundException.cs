using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.ProjectUsersExceptions
{
    public sealed class ProjectUsersNotFoundException : NotFoundException
    {
        public ProjectUsersNotFoundException(Guid userId, Guid projectId) : base($"The Pair with the identifier userId : {userId} and identifier projectId : {projectId} not found.") { }
    }
}
