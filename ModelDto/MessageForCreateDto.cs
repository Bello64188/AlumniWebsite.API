using AlumniWebsite.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ModelDto
{
    public class MessageForCreateDto
    {
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string SenderKnownAs { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }
        public MessageForCreateDto()
        {
            MessageSent = DateTime.Now;
        }
    }
}
