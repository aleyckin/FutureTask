﻿using Domain.Entities;
using Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class ProjectUsersRepository : IProjectUsersRepository
    {
        private readonly RepositoryDbContext _dbContext;

        public ProjectUsersRepository(RepositoryDbContext dbContext) { _dbContext = dbContext; }

        public Task<List<Project>> GetAllProjectsByUser(Guid UserId, CancellationToken cancellationToken = default)
        {
            return _dbContext.ProjectUsers
                .Where(x => x.UserId == UserId)
                .Include(projects => projects.Project)
                .Select(projects => projects.Project)
                .ToListAsync(cancellationToken);
        }

        public Task<List<User>> GetAllUsersByProject(Guid ProjectId, CancellationToken cancellationToken = default)
        {
            return _dbContext.ProjectUsers
                .Where(x => x.ProjectId == ProjectId)
                .Include(users => users.User)
                .Select(users => users.User)
                .ToListAsync(cancellationToken);
        }

        public Task<ProjectUsers> GetProjectUser(Guid UserId, Guid ProjectId, CancellationToken  cancellationToken = default)
        {
            return _dbContext.ProjectUsers
                .FirstOrDefaultAsync(x => x.UserId == UserId && x.ProjectId == ProjectId, cancellationToken);
        }

        public void Insert(ProjectUsers projectUsers)
        {
            _dbContext.Add(projectUsers);
        }

        public void Remove(ProjectUsers projectUsers)
        {
            _dbContext.Remove(projectUsers);
        }
    }
}
