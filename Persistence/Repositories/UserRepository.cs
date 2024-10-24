using Domain.Entities;
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
    public class UserRepository : IUserRepository
    {
        private readonly RepositoryDbContext _dbContext;

        public UserRepository(RepositoryDbContext dbContext)  { _dbContext = dbContext; }

        public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(x => x.Specialization)
                .Include(x => x.Tasks)
                .Include(x => x.ProjectUsers)
                .ThenInclude(x => x.Project)
                .ToListAsync(cancellationToken);
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(x => x.Specialization)
                .Include(x => x.Tasks)
                .Include(x => x.ProjectUsers)
                .ThenInclude(x => x.Project)
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(x => x.Specialization)
                .Include(x => x.Tasks)
                .Include(x => x.ProjectUsers)
                .ThenInclude(x => x.Project)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Insert(User user)
        {
            _dbContext.Add(user);
        }

        public void Remove(User user)
        {
            _dbContext.Remove(user);
        }
    }
}
