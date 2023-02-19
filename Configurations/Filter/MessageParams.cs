using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Configurations.Filter
{
    public class MessageParams
    {
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize ? MaxPageSize : value); }
        }

        private const int MaxPageSize = 50;
        public string MemberId { get; set; }
        public string MessageContainer { get; set; } = "unread";
    }
}
