using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ModelDto
{
    public class DocumentDto
    {
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }
}
