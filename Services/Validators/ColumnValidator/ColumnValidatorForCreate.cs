using Contracts.Dtos.ColumnDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.ColumnValidator
{
    public class ColumnValidatorForCreate : AbstractValidator<ColumnDtoForCreate>
    {
        public ColumnValidatorForCreate() 
        {
            RuleFor(column => column.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("The title must consists less or equal 200 characters.");
            RuleFor(column => column.ProjectId)
                .NotEmpty().WithMessage("ProjectId is required.");
        }
    }
}
