using Contracts.Dtos.UserDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validators.UserValidators
{
    public class UserValidatorForUpdate : AbstractValidator<UserDtoForUpdate>
    {
        public UserValidatorForUpdate() 
        {
            RuleFor(user => user.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .When(user => user.Email != null);

            RuleFor(user => user.Password)
                .MinimumLength(6).WithMessage("Password must be 6 characters at least.")
                .When(user => user.Password != null);        

            RuleFor(user => user.SpecializationId)
                .NotEmpty().WithMessage("SpecializationId is required.")
                .When(user => user.SpecializationId.HasValue);
        }
    }
}
