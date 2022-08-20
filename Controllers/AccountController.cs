using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using AlumniWebsite.API.ModelDto;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthManager _authManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountController(UserManager<Member> userManager,
            ILogger<AccountController> logger, IMapper mapper, IAuthManager auth, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _logger = logger;
            _authManager = auth;
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            _logger.LogInformation($"attempt to register{registerDto.Email}");
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            if (await UserExists(registerDto.Email))
            {
                return BadRequest("Email Address is Taken");
            }

            var member = _mapper.Map<Member>(registerDto);
            member.Email = registerDto.Email.ToLower();
            var success = await _userManager.CreateAsync(member, registerDto.Password);
            if (!success.Succeeded) return BadRequest(success.Errors);
            var memberRole = await _userManager.AddToRoleAsync(member, "Member");
            if (!memberRole.Succeeded) return BadRequest(memberRole.Errors);
            var returnToMemeber = _mapper.Map<MemberDetailsDto>(member);
            return CreatedAtRoute("GetMember", new { controller = "Members", id = member.Id }, returnToMemeber);

        }



        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var member = await _unitOfWork.MemberRepository.Login(loginDto.Email.ToLower(), loginDto.Password);
            _logger.LogInformation($"Login Attempt for {loginDto.Email}");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _authManager.ValidateUser(loginDto)) return Unauthorized();
            if (member == null) return NotFound();
            var memberFromDto = _mapper.Map<MemberListDto>(member);
            return Ok(new
            {
                Token = await _authManager.CreateToken(),
                memberFromDto = memberFromDto

            });
        }

        private async Task<bool> UserExists(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
