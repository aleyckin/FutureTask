using AutoMapper;
using Contracts.Dtos.ProjectDtos;
using Domain.RepositoryInterfaces;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IRepositoryManager repositoryManager;
        private readonly IMapper mapper;

        public ProjectService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            this.repositoryManager = repositoryManager;
            this.mapper = mapper;
        }

        public Task<ProjectDto> CreateAsync(ProjectDtoForCreate projectDtoForCreate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDto> GetProjectById(Guid projectId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid projectId, ProjectDtoForUpdate projectDtoForUpdate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
