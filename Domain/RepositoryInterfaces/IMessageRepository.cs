using Domain.Entities;
using Domain.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetAllMessagesForTaskAsync(Guid TaskId, CancellationToken cancellationToken = default);
        void Insert(Message message);
        void Remove(Message message);
    }
}
