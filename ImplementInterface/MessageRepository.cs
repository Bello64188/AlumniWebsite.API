using AlumniWebsite.API.Configurations.Filter;
using AlumniWebsite.API.Data;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ImplementInterface
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<PagedList<Message>> GetMessageForUser(MessageParams messageParams)
        {
            var message = _context.Messages
                 .Include(m => m.Sender).ThenInclude(m => m.Photos)
                 .Include(m => m.Recipient).ThenInclude(m => m.Photos)
                 .AsQueryable();
            switch (messageParams.MessageContainer)
            {
                case "inbox":
                    message = message.Where(u => u.RecipientId == messageParams.MemberId && u.RecipientDeleted == false);
                    break;
                case "outbox":
                    message = message.Where(m => m.SenderId == messageParams.MemberId && m.SenderDeleted == false);
                    break;
                default:
                    message = message.Where(u => u.RecipientId == messageParams.MemberId && u.RecipientDeleted == false && u.IsRead == false);
                    break;
            }
            message = message.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>.CreatAsync(message, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(string userid, string recipientid)
        {
            var message = await _context.Messages
                 .Include(m => m.Sender).ThenInclude(m => m.Photos)
                 .Include(u => u.Recipient).ThenInclude(u => u.Photos)
                 .Where(m => m.SenderId == userid && m.RecipientId == recipientid && m.SenderDeleted == false
                 || m.SenderId == recipientid && m.RecipientId == userid && m.RecipientDeleted == false)
                 .OrderByDescending(m => m.MessageSent)
                 .ToListAsync();
            return message;
        }
    }
}
