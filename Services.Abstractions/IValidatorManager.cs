using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IValidatorManager
    {
        Task ValidateAsync<T>(T instance, CancellationToken cancellationToken = default);
    }
}
