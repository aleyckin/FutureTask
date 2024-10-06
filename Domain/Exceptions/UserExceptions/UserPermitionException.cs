using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.UserExceptions
{
    public sealed class UserPermitionException : PermitionException
    {
        public UserPermitionException() : base("You don't have permition for this action.") { }
    }
}
