using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IRepositoryManager
    {
        IProjectRepository ProjectRepository { get; }
        IUserRepository UserRepository { get; }
        IColumnRepository ColumnRepository { get; }
        ISpecializationRepository SpecializationRepository { get; }
        ITaskRepository TaskRepository { get; }
        IProjectUsersRepository ProjectUsersRepository { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
