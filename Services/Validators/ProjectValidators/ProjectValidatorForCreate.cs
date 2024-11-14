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
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(1).WithMessage("Name is shorter than 1 chars.")
                .MaximumLength(200).WithMessage("Name is longer than 200 chars.");
        }
    }
}
