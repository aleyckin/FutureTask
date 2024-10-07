using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators
{
    public class ValidatorManager : IValidatorManager
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidatorManager(IServiceProvider serviceProvider) { _serviceProvider = serviceProvider; }

        public async Task ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();
            if (validator == null)
            {
                throw new InvalidOperationException($"Валидатор для типа {typeof(T).Name} не зарегистрирован.");
            }

            var validateResult = await validator.ValidateAsync(instance, cancellationToken);
            if (!validateResult.IsValid)
            {
                throw new ValidationException(validateResult.Errors);
            }
        }
    }
}
