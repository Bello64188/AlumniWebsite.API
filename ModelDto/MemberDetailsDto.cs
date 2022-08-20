using AlumniWebsite.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ModelDto
{
    public class MemberDetailsDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime GraduationYear { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string PhoneNumber { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}
