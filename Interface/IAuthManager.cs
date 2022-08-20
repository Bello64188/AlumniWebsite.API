using AlumniWebsite.API.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Interface
{
    public interface IAuthManager
    {
        public Task<bool> ValidateUser(LoginDto loginDto);
        public Task<string> CreateToken();
    }
}
