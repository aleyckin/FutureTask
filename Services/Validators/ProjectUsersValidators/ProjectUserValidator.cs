using Contracts.Dtos.ColumnDtos;
using Contracts.Dtos.ProjectUsersDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.ProjectUsersValidators
{
    public class ProjectUserValidator : AbstractValidator<ProjectUsersDto>
    {
        public ProjectUserValidator() 
        {
            RuleFor(projectUsers => projectUsers.RoleOnProject)
                .IsInEnum().WithMessage("RoleOnProject must be DefaultWorker or TeamLead.");
        }
    }
}
