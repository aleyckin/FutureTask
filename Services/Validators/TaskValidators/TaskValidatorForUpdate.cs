using FluentValidation;
using Contracts.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Dtos.TaskDtos;

namespace Services.Validators.TaskValidators
{
    public class TaskValidatorForUpdate : AbstractValidator<TaskDtoForUpdate>
    {
        public TaskValidatorForUpdate() 
        {
            RuleFor(task => task.Title)
                .MaximumLength(200).WithMessage("The title must consist of less or equal to 200 characters.")
                .When(task => task.Title != null);

            RuleFor(task => task.Description)
                .MaximumLength(500).WithMessage("The description must consist of less or equal to 500 characters.")
                .When(task => task.Description != null);

            RuleFor(task => task.Priority)
                .IsInEnum().WithMessage("Priority must be 'Low', 'Medium', or 'High'.")
                .When(task => task.Priority.HasValue);

            RuleFor(task => task.DateEnd)
                .GreaterThan(DateTime.UtcNow).WithMessage("End date must be in the future.")
                .When(task => task.DateEnd.HasValue);

            RuleFor(task => task.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .When(task => task.UserId.HasValue);

            RuleFor(task => task.ColumnId)
                .NotEmpty().WithMessage("ColumnId is required.")
                .When(task => task.ColumnId.HasValue);
        }
    }
}
