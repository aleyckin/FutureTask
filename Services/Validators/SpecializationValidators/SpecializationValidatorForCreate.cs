using Contracts.Dtos.SpecializationDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.SpecializationValidators
{
    public class SpecializationValidatorForCreate : AbstractValidator<SpecializationDtoForCreate>
    {
        public SpecializationValidatorForCreate()
        {
            RuleFor(specialization => specialization.Name)
                .NotEmpty().WithMessage("Name is required.");
        }
    }
}
