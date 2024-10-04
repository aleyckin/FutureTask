using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.AbstractExceptions
{
    public abstract class BadCredentialsException : Exception
    {
        protected BadCredentialsException(string message) : base(message) { }
    }
}
