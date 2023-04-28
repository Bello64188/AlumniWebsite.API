using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ModelDto
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email requried.")]
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Password and confirm password did not match.")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }

    }
}
