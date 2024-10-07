using Contracts.Dtos.ColumnDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.ColumnValidator
{
    public class ColumnValidatorForUpdate : AbstractValidator<ColumnDtoForUpdate>
    {
        public ColumnValidatorForUpdate() 
        {
            RuleFor(column => column.Title)
                .MaximumLength(200).WithMessage("The title must consists less or equal 200 characters.")
                .When(task => task.Title != null);
        }
    }
}
