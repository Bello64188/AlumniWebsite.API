using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Model
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public virtual Member Sender { get; set; }
        public string RecipientId { get; set; }
        public virtual Member Recipient { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }

    }
}
