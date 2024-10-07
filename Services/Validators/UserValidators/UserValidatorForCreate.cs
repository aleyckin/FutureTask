using Contracts.Dtos.UserDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.UserValidators
{
    public class UserValidatorForCreate : AbstractValidator<UserDtoForCreate>
    {
        public UserValidatorForCreate()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Name is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be 6 characters at least.");

            RuleFor(user => user.UserRole)
                .IsInEnum().WithMessage("UserRole must be either RegularUser or Administrator.");

            RuleFor(user => user.SpecializationId)
                .NotEmpty().WithMessage("SpecializationId is required.");
        }
    }
}
