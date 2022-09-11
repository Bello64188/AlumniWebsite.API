using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Model
{
    public class Like
    {
        public string LikerId { get; set; }
        public string LikeeId { get; set; }
        public Member Liker { get; set; }
        public Member Likee { get; set; }

    }
}
