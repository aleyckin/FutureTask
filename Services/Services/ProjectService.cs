using AutoMapper;
using Contracts.Dtos.ProjectDtos;
using Domain.Entities;
using Domain.Exceptions.ProjectExceptions;
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
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public ProjectService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<ProjectDto> CreateAsync(ProjectDtoForCreate projectDtoForCreate, CancellationToken cancellationToken = default)
        {
            var project = _mapper.Map<Project>(projectDtoForCreate);
            _repositoryManager.ProjectRepository.Insert(project);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ProjectDto>(project);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var project = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(projectId);
            }
            _repositoryManager.ProjectRepository.Remove(project);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var projects = await _repositoryManager.ProjectRepository.GetAllProjectsAsync(cancellationToken);
            return _mapper.Map<List<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> GetProjectById(Guid projectId, CancellationToken cancellationToken = default)
        {
            var project = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(projectId, cancellationToken);
            if (project == null) 
            {
                throw new ProjectNotFoundException(projectId);
            }
            return _mapper.Map<ProjectDto>(project);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid projectId, ProjectDtoForUpdate projectDtoForUpdate, CancellationToken cancellationToken = default)
        {
            var project = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(projectId);
            }
            if (projectDtoForUpdate.Name != null)
            {
                project.Name = projectDtoForUpdate.Name;
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
