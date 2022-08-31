using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Configurations.Filter
{
    public class MemberParams
    {
        //page parameter
        public int PageNumber { get; set; } = 1;
        private int pageSize = 5;
        private const int MaxPageSize = 50;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize ? MaxPageSize : value); }
        }
        //filter
        public string MemberId { get; set; }
        //public int MinAge { get; set; } = 18;
        //public int MaxAge { get; set; } = 99;
        public string Gender { get; set; }
        public int? GraduationYear { get; set; }
        //sorting
        public string OrderBy { get; set; } = "lastactive";
        public bool Likers { get; set; } = false;
        public bool Likees { get; set; } = false;




    }
}
