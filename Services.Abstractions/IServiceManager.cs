using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IServiceManager
    {
        IProjectService ProjectService { get; }
        ISpecializationService SpecializationService { get; }
        IUserService UserService { get; }
        IColumnService ColumnService { get; }
        ITaskService TaskService { get; }
    }
}
