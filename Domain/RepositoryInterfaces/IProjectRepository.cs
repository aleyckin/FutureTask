using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllProjectsAsync(CancellationToken cancellationToken);
        Task<Project> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken);
        void Insert(Project project);
        void Remove(Project project);
    }
}
