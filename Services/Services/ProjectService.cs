using AutoMapper;
using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.UserDtos;
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
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public ProjectService(IProjectRepository projectRepository, IUnitOfWork unitOfWork, IMapper mapper, IValidatorManager validatorManager)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async Task<ProjectDto> CreateAsync(ProjectDtoForCreate projectDtoForCreate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(projectDtoForCreate, cancellationToken);

            var project = _mapper.Map<Project>(projectDtoForCreate);
            _projectRepository.Insert(project);
            Column columnForTest = new Column
            {
                ProjectId = project.Id,
                Title = "Готово к тестированию"
            };
            Column columnForDevelop = new Column
            {
                ProjectId = project.Id,
                Title = "В разработке"
            };
            Column columnForPlanning = new Column
            {
                ProjectId = project.Id,
                Title = "Стадия планирования"
            };
            project.Columns.Add(columnForTest);
            project.Columns.Add(columnForDevelop);
            project.Columns.Add(columnForPlanning);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ProjectDto>(project);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetProjectByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(projectId);
            }
            _projectRepository.Remove(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var projects = await _projectRepository.GetAllProjectsAsync(cancellationToken);
            return _mapper.Map<List<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> GetProjectById(Guid projectId, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetProjectByIdAsync(projectId, cancellationToken);
            if (project == null) 
            {
                throw new ProjectNotFoundException(projectId);
            }
            return _mapper.Map<ProjectDto>(project);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid projectId, ProjectDtoForUpdate projectDtoForUpdate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(projectDtoForUpdate, cancellationToken);

            var project = await _projectRepository.GetProjectByIdAsync(projectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(projectId);
            }

            _mapper.Map(projectDtoForUpdate, project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
