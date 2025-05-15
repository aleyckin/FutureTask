using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IMetricsService
    {
        Task<byte[]> GetReportForProject(Guid ProjectId);
        Task<byte[]> GetReportForUser(Guid UserId);
    }
}
