using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ModelDto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Please Enter Your Email Address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Strong Password")]
        public string Password { get; set; }
    }
}
