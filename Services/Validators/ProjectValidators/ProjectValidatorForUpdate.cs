using Contracts.Dtos.ProjectDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.ProjectValidators
{
    public class ProjectValidatorForUpdate : AbstractValidator<ProjectDtoForUpdate>
    {
        public ProjectValidatorForUpdate() 
        {
            RuleFor(project => project.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name is longer than 200 chars.")
                .When(project => project.Name != null);
        }
    }
}
