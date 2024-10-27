using Domain.Entities.Helpers;
using Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly RepositoryDbContext _dbContext;

        public MessageRepository(RepositoryDbContext dbContext) { _dbContext = dbContext; }

        public async Task<List<Message>> GetAllMessagesForTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Messages
                .Where(m => m.TaskId == taskId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public void Insert(Message message)
        {
            _dbContext.Add(message);
        }

        public void Remove(Message message)
        {
            _dbContext.Remove(message);
        }
    }
}
