using AutoMapper;
using Domain.RepositoryInterfaces;
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
        private readonly Lazy<IAdministratorService> _lazyAdministratorService;
        private readonly Lazy<IProjectService> _lazyProjectService;
        private readonly Lazy<ISpecializationService> _lazySpecialization;
        private readonly Lazy<IUserService> _lazyUser;

        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)
        { 
            _lazyAdministratorService = new Lazy<IAdministratorService>(() => new AdministratorService(repositoryManager, mapper));
            _lazyProjectService = new Lazy<IProjectService>(() => new ProjectService(repositoryManager, mapper));
            _lazySpecialization = new Lazy<ISpecializationService>(() => new SpecializationService(repositoryManager, mapper));
            _lazyUser = new Lazy<IUserService>(() => new UserService(repositoryManager, mapper));
        }

        public IAdministratorService AdministratorService => _lazyAdministratorService.Value;
        public IProjectService ProjectService => _lazyProjectService.Value;
        public ISpecializationService SpecializationService => _lazySpecialization.Value;
        public IUserService UserService => _lazyUser.Value;
    }
}
