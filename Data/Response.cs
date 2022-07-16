using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Data
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string Messages { get; set; }
         public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
   
}
