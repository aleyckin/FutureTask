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

        private IAdministratorRepository _administratorRepository;
        private IProjectRepository _projectRepository;

        public RepositoryManager(RepositoryDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public IAdministratorRepository AdministratorRepository => _administratorRepository ??= new AdministratorRepository(_dbContext);

        public IProjectRepository ProjectRepository => _projectRepository ??= new ProjectRepository(_dbContext);

        public IUnitOfWork UnitOfWork => _unitOfWork;
    }
}
