using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IAdministratorRepository
    {
        Task<List<Administrator>> GetAllAdministratorsAsync(CancellationToken cancellationToken = default);
        Task<Administrator> GetAdministratorByIdAsync(Guid id,CancellationToken cancellationToken = default);
        void Insert(Administrator administator);
        void Remove(Administrator administrator);
    }
}
