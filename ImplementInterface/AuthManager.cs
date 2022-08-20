using AlumniWebsite.API.Data;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using AlumniWebsite.API.ModelDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ImplementInterface
{
    public class AuthManager : IAuthManager
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Member> _userManager;
        private readonly AppDbContext _context;
        private DbSet<Member> _db;
        private Member _member;

        public AuthManager(UserManager<Member> userManager, IConfiguration configuration,
            AppDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _db = _context.Set<Member>();

        }
        public async Task<string> CreateToken()
        {
            var claim = await GetClaimsAsync();
            var signingCredential = GetSigning();
            var token = GetTokenOption(signingCredential, claim);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //getting token Value
        private JwtSecurityToken GetTokenOption(SigningCredentials signing, List<Claim> claim)
        {
            var jwtSetting = _configuration.GetSection("Jwt");
            var expiration = DateTime.Now.AddHours(Convert.ToDouble(jwtSetting.GetSection("Lifetime").Value));
            var token = new JwtSecurityToken(
                issuer: jwtSetting.GetSection("Issuer").Value,
                claims: claim,
                expires: expiration,
                signingCredentials: signing
                );
            return token;
        }

        private async Task<List<Claim>> GetClaimsAsync()
        {
            var claim = new List<Claim>
            {
                new Claim("userName", _member.UserName),
                new Claim("id",_member.Id.ToString())
        };

            var role = await _userManager.GetRolesAsync(_member);
            claim.AddRange(role.Select(roles => new Claim(ClaimTypes.Role, roles)));
            return claim;
        }

        private SigningCredentials GetSigning()
        {
            var key = Environment.GetEnvironmentVariable("KEYAPI");
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        public async Task<bool> ValidateUser(LoginDto loginDto)
        {
            _member = await _userManager.FindByEmailAsync(loginDto.Email);
            return (_member != null && await _userManager.CheckPasswordAsync(_member, loginDto.Password));
        }


    }
}
