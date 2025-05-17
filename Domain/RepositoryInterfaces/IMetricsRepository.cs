using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IMetricsRepository
    {
        System.Threading.Tasks.Task AddAsync(TaskMetrics metrics);
    }
}
