using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IRepositoryManager
    {
        IAdministratorRepository AdministratorRepository { get; }
        IProjectRepository ProjectRepository { get; }
        IUserRepository UserRepository { get; }
        IColumnRepository ColumnRepository { get; }
        ISpecializationRepository SpecializationRepository { get; }
        ITaskRepository TaskRepository { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
