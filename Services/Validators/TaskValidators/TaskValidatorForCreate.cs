using Contracts.Dtos.TaskDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.TaskValidators
{
    public class TaskValidatorForCreate : AbstractValidator<TaskDtoForCreate>
    {
        public TaskValidatorForCreate() 
        {
            RuleFor(task => task.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("The title must consists less or equal 200 characters.");

            RuleFor(task => task.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(task => task.Priority)
                .IsInEnum().WithMessage("Priority must be 'Low', 'Medium' or 'High'.");

            RuleFor(task => task.DateEnd)
                .NotEmpty().WithMessage("DateEnd is required.");

            RuleFor(task => task.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(task => task.ColumnId)
                .NotEmpty().WithMessage("ColumnId is required.");
        }
    }
}
