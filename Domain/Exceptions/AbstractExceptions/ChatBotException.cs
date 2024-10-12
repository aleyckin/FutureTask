using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.AbstractExceptions
{
    public abstract class ChatBotException : Exception
    {
        protected ChatBotException(string message) : base(message) { }
    }
}
