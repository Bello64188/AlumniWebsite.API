using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Model
{
    public class Photo
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Url { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public bool IsApproved { get; set; }
        [MaxLength(200)]
        public string PublicId { get; set; }
        public Member Member { get; set; }
        [ForeignKey("Member")]
        public string MemberId { get; set; }

    }
}
