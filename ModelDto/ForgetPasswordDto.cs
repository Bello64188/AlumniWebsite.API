using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ModelDto
{
    public class ForgetPasswordDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string ClientUrl { get; set; }
    }
}
