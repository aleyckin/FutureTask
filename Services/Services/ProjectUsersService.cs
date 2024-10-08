﻿using AutoMapper;
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
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public ProjectUsersService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async System.Threading.Tasks.Task AddUserToProjectAsync(ProjectUsersDto projectUsersDto, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(projectUsersDto, cancellationToken);

            var project = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(projectUsersDto.ProjectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(projectUsersDto.ProjectId);
            }

            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(projectUsersDto.UserId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(projectUsersDto.UserId);
            }

            var projectUser = _mapper.Map<ProjectUsers>(projectUsersDto);

            _repositoryManager.ProjectUsersRepository.Insert(projectUser);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task DeleteUserFromProjectAsync(ProjectUsersDto projectUsersDto, CancellationToken cancellationToken = default)
        {
            var projectUser = await _repositoryManager.ProjectUsersRepository.GetProjectUser(projectUsersDto.UserId, projectUsersDto.ProjectId, cancellationToken);
            if (projectUser == null)
            {
                throw new ProjectUsersNotFoundException(projectUsersDto.UserId, projectUsersDto.ProjectId);
            }
            _repositoryManager.ProjectUsersRepository.Remove(projectUser);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ProjectUsersDtoForListProjects>> GetAllProjectsByUser(Guid UserId, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetUserByIdAsync(UserId, cancellationToken);
            if (user == null) 
            { 
                throw new UserNotFoundException(UserId);
            }
            var projects = await _repositoryManager.ProjectUsersRepository.GetAllProjectsByUser(UserId, cancellationToken);
            return _mapper.Map<List<ProjectUsersDtoForListProjects>>(projects);
        }

        public async Task<List<ProjectUsersDtoForListUsers>> GetAllUsersByProject(Guid ProjectId, CancellationToken cancellationToken = default)
        {
            var project = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(ProjectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(ProjectId);
            }
            var users = await _repositoryManager.ProjectUsersRepository.GetAllUsersByProject(ProjectId, cancellationToken);
            return _mapper.Map<List<ProjectUsersDtoForListUsers>>(users);
        }

        public async System.Threading.Tasks.Task UpdateUserRoleInProjectAsync(ProjectUsersDto projectUsersDto, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(projectUsersDto, cancellationToken);

            var projectUser = await _repositoryManager.ProjectUsersRepository.GetProjectUser(projectUsersDto.UserId, projectUsersDto.ProjectId, cancellationToken);
            if (projectUser == null)
            {
                throw new ProjectUsersNotFoundException(projectUsersDto.UserId, projectUsersDto.ProjectId);
            }
            if (projectUsersDto.RoleOnProject != null)
            {
                projectUser.RoleOnProject = projectUsersDto.RoleOnProject.Value;
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<RoleOnProject?> GetUserRoleOnProject(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
        {
            var projectUser = await _repositoryManager.ProjectUsersRepository.GetProjectUser(userId, projectId, cancellationToken);
            if (projectUser == null)
            {
                throw new ProjectUsersNotFoundException(userId, projectId);
            }
            return projectUser.RoleOnProject;
        }
    }
}
