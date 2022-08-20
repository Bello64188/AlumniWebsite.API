using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ModelDto
{
    public class RegisterDto : LoginDto
    {
        public RegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        [Required]
        public DateTime GraduationYear { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
