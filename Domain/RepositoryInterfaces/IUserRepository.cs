using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        void Insert(User user);
        void Remove(User user);
    }
}
