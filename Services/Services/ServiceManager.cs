using AutoMapper;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IProjectService> _lazyProjectService;
        private readonly Lazy<ISpecializationService> _lazySpecialization;
        private readonly Lazy<IUserService> _lazyUser;
        private readonly Lazy<IColumnService> _lazyColumn;
        private readonly Lazy<ITaskService> _lazyTask;
        private readonly Lazy<IProjectUsersService> _lazyProjectUsers;

        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper, IConfiguration configuration)
        { 
            _lazyProjectService = new Lazy<IProjectService>(() => new ProjectService(repositoryManager, mapper));
            _lazySpecialization = new Lazy<ISpecializationService>(() => new SpecializationService(repositoryManager, mapper));
            _lazyUser = new Lazy<IUserService>(() => new UserService(repositoryManager, mapper, configuration));
            _lazyColumn = new Lazy<IColumnService>(() => new ColumnService(repositoryManager, mapper));
            _lazyTask = new Lazy<ITaskService>(() => new TaskService(repositoryManager, mapper));
            _lazyProjectUsers = new Lazy<IProjectUsersService>(() => new ProjectUsersService(repositoryManager, mapper));
        }

        public IProjectService ProjectService => _lazyProjectService.Value;
        public ISpecializationService SpecializationService => _lazySpecialization.Value;
        public IUserService UserService => _lazyUser.Value;
        public IColumnService ColumnService => _lazyColumn.Value;
        public ITaskService TaskService => _lazyTask.Value;
        public IProjectUsersService ProjectUsersService => _lazyProjectUsers.Value;
    }
}
