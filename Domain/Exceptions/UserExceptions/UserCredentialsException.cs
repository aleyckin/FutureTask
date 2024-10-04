using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.UserExceptions
{
    public sealed class UserCredentialsException : BadCredentialsException
    {
        public UserCredentialsException() : base("Wrong password.") { }
    }
}
