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
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly RepositoryDbContext _dbContext;
        public SpecializationRepository(RepositoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Specialization>> GetAllSpecializationsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Specializations
                .ToListAsync(cancellationToken);
        }

        public async Task<Specialization> GetSpecializationByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Specializations
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Specialization> GetSpecializationByNameAsync(string Name, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Specializations
                .FirstOrDefaultAsync(x => x.Name == Name, cancellationToken);
        }

        public void Insert(Specialization specialization)
        {
            _dbContext.Add(specialization);
        }

        public void Remove(Specialization specialization)
        {
            _dbContext.Remove(specialization);
        }
    }
}
