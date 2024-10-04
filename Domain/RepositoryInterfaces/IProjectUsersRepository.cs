using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IProjectUsersRepository
    {
        Task<List<User>> GetAllUsersByProject(Guid ProjectId, CancellationToken cancellationToken = default);
        Task<List<Project>> GetAllProjectsByUser(Guid UserId, CancellationToken cancellationToken = default);
        Task<ProjectUsers> GetProjectUser(Guid UserId, Guid ProjectId, CancellationToken cancellationToken = default);
        void Insert(ProjectUsers projectUsers);
        void Remove(ProjectUsers projectUsers);
    }
}
