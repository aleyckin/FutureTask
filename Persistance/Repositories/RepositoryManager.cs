using Domain.RepositoryInterfaces;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        private IProjectRepository _projectRepository;
        private ISpecializationRepository _specializationRepository;
        private IUserRepository _userRepository;
        private IColumnRepository _columnRepository;
        private ITaskRepository _taskRepository;

        public RepositoryManager(RepositoryDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public ISpecializationRepository SpecializationRepository => _specializationRepository ??= new SpecializationRepository(_dbContext);

        public IProjectRepository ProjectRepository => _projectRepository ??= new ProjectRepository(_dbContext);

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_dbContext);

        public IColumnRepository ColumnRepository => _columnRepository ??= new ColumnRepository(_dbContext);
        public ITaskRepository TaskRepository => _taskRepository ??= new TaskRepository(_dbContext);

        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}
