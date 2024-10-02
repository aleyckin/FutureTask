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

        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)
        { 
            _lazyAdministratorService = new Lazy<IAdministratorService>(() => new AdministratorService(repositoryManager, mapper));
        }

        public IAdministratorService AdministratorService => _lazyAdministratorService.Value;
    }
}
