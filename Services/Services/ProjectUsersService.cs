using AutoMapper;
using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.ProjectUsersDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Exceptions.ProjectExceptions;
using Domain.Exceptions.ProjectUsersExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.RepositoryInterfaces;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProjectUsersService : IProjectUsersService
    {
        private readonly IProjectUsersRepository _projectUsersRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public ProjectUsersService(
            IProjectUsersRepository projectUsersRepository,
            IProjectRepository projectRepository, 
            IUserRepository userRepository,
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IValidatorManager validatorManager)
        {
            _projectUsersRepository = projectUsersRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async System.Threading.Tasks.Task AddUserToProjectAsync(ProjectUsersDto projectUsersDto, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(projectUsersDto, cancellationToken);

            var project = await _projectRepository.GetProjectByIdAsync(projectUsersDto.ProjectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(projectUsersDto.ProjectId);
            }

            var user = await _userRepository.GetUserByIdAsync(projectUsersDto.UserId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(projectUsersDto.UserId);
            }

            var projectUser = _mapper.Map<ProjectUsers>(projectUsersDto);

            _projectUsersRepository.Insert(projectUser);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task DeleteUserFromProjectAsync(Guid UserId, Guid ProjectId, CancellationToken cancellationToken = default)
        {
            var projectUser = await _projectUsersRepository.GetProjectUser(UserId, ProjectId, cancellationToken);
            if (projectUser == null)
            {
                throw new ProjectUsersNotFoundException(UserId, ProjectId);
            }
            _projectUsersRepository.Remove(projectUser);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ProjectUsersDtoForListProjects>> GetAllProjectsByUser(Guid UserId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(UserId, cancellationToken);
            if (user == null) 
            { 
                throw new UserNotFoundException(UserId);
            }
            var projects = await _projectUsersRepository.GetAllProjectsByUser(UserId, cancellationToken);
            return _mapper.Map<List<ProjectUsersDtoForListProjects>>(projects);
        }

        public async Task<List<ProjectUsersDtoForListProjects>> GetAllProjectsAsAdmin(CancellationToken cancellationToken = default)
        {
            var projects = await _projectUsersRepository.GetAllProjectsAsAdmin(cancellationToken);
            return _mapper.Map<List<ProjectUsersDtoForListProjects>>(projects);
        }

        public async Task<List<ProjectUsersDtoForListUsers>> GetAllUsersByProject(Guid ProjectId, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetProjectByIdAsync(ProjectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(ProjectId);
            }
            var users = await _projectUsersRepository.GetAllUsersByProject(ProjectId, cancellationToken);
            return _mapper.Map<List<ProjectUsersDtoForListUsers>>(users);
        }

        public async System.Threading.Tasks.Task UpdateUserRoleInProjectAsync(ProjectUsersDto projectUsersDto, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(projectUsersDto, cancellationToken);

            var projectUser = await _projectUsersRepository.GetProjectUser(projectUsersDto.UserId, projectUsersDto.ProjectId, cancellationToken);
            if (projectUser == null)
            {
                throw new ProjectUsersNotFoundException(projectUsersDto.UserId, projectUsersDto.ProjectId);
            }
            if (projectUsersDto.RoleOnProject != null)
            {
                projectUser.RoleOnProject = projectUsersDto.RoleOnProject.Value;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<RoleOnProject?> GetUserRoleOnProject(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
        {
            var projectUser = await _projectUsersRepository.GetProjectUser(userId, projectId, cancellationToken);
            if (projectUser == null)
            {
                throw new ProjectUsersNotFoundException(userId, projectId);
            }
            return projectUser.RoleOnProject;
        }
    }
}
