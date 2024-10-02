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
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly RepositoryDbContext _dbContext;
        public AdministratorRepository(RepositoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Administrator>> GetAllAdministratorsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Administrators.Include(x => x.Projects).ToListAsync(cancellationToken);
        }

        public async Task<Administrator> GetAdministratorByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Administrators.Include(x => x.Projects).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Insert(Administrator administator)
        {
            _dbContext.Add(administator);
        }

        public void Remove(Administrator administrator)
        {
            _dbContext.Remove(administrator);
        }
    }
}
