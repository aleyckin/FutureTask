using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.TaskExceptions
{
    public sealed class ChatBotUnavailableException : ChatBotException
    {
        public ChatBotUnavailableException() : base("ChatBot is unavailable at this moment.") { }
    }
}
