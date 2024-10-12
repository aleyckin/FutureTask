using Domain.Exceptions.AbstractExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.TaskExceptions
{
    public sealed class ChatBotContextException : ChatBotException
    {
        public ChatBotContextException(Guid taskId) : base($"Context of ChatBot on task with identifier {taskId} is empty.") { }
    }
}
