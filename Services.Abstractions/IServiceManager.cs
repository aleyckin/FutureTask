using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IServiceManager
    {
        IAdministratorService AdministratorService { get; }
        IProjectService ProjectService { get; }
        ISpecializationService SpecializationService { get; }
        IUserService UserService { get; }
        IColumnService ColumnService { get; }
    }
}
