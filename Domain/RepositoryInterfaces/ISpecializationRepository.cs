using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface ISpecializationRepository
    {
        Task<List<Specialization>> GetAllSpecializationsAsync(CancellationToken cancellationToken = default);
        Task<Specialization> GetSpecializationByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Insert(Specialization specialization);
        void Remove(Specialization specialization);
    }
}
