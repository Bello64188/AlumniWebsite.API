using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Model
{
    public class Member : IdentityUser
    {
        public Member()
        {
            Photos = new Collection<Photo>();
        }
        [MaxLength(100)]
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(100)]
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        [MaxLength(4)]
        public int GraduationYear { get; set; }
        [MaxLength(500)]
        public string Introduction { get; set; }
        [MaxLength(500)]
        public string LookingFor { get; set; }
        [MaxLength(500)]
        public string Interests { get; set; }
        [MaxLength(150)]
        public string City { get; set; }
        [MaxLength(150)]
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Like> Likers { get; set; }
        public ICollection<Like> Likees { get; set; }
        public ICollection<Message> MessageSent { get; set; }
        public ICollection<Message> MessageReceived { get; set; }
        public ICollection<MemberRole> MemberRoles { get; set; }

    }
}
