using Contracts.Dtos.ProjectDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.ProjectValidators
{
    public class ProjectValidatorForCreate : AbstractValidator<ProjectDtoForCreate>
    {
        public ProjectValidatorForCreate() 
        {
            RuleFor(project => project.Name)
                .NotEmpty().WithMessage("Name is required.");
        }
    }
}
