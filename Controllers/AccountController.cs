using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using AlumniWebsite.API.ModelDto;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly IEmailService emailService;
        private readonly IMapper _mapper;
        private CancellationToken ct;

        public AccountController(UserManager<Member> userManager,
            ILogger<AccountController> logger,
            IMapper mapper, IAuthManager auth, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _userManager = userManager;
            _logger = logger;
            _authManager = auth;
            _unitOfWork = unitOfWork;
            this.emailService = emailService;
            _mapper = mapper;

        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            _logger.LogInformation($"attempt to register  {registerDto.Email}");
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
        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
        {

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            var member = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
            if (member == null)
            {
                return NotFound("Email not found");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(member);
            var param = new Dictionary<string, string>
            {
                {"token",token },
                {"email",forgetPasswordDto.Email }
            };
            var callback = QueryHelpers.AddQueryString(forgetPasswordDto.ClientUrl, param);
            var mailReq = new EmailRequest();
            mailReq.ToEmail = member.Email;
            mailReq.Subject = "Reset password token";
            mailReq.Body = $"<div style='max-width:600px; margin-right:auto;margin-left:auto;'>" +
                $"<div style='text-align: center;color: white;font-size: 25px;padding: 25px; background: #4582ec; border-radius:5px;'>" +
                $"<p>Modebe Memorial Secondary School Onitsha</p></div>" +
                $"<div style='text-align: center; margin-top:5px; font-weight:600;font-size: 18px;'> Dear {member.UserName}, </div>" +
                $"<div style='text-align: center; margin-top:5px;font-size: 18px;'> Please use below link to reset your password. </div>" +
                $"<div style='text-align:center; margin-top:30px;'>" +
                $"<a style='background-color: #199319;color: white;padding: 10px 15px;text-decoration: none; border-radius:3px;' href='{callback}'>Reset your password</a></div>" +
                $"</div>";
            await emailService.SendEmail(mailReq, ct);
            return Ok();
        }
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {

            if (!ModelState.IsValid)
                return BadRequest("Invalid request.");
            var member = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (member == null)
                return NotFound("Request not found.");
            var resetPassword = await _userManager.ResetPasswordAsync(member, resetPasswordDto.Token, resetPasswordDto.Password);
            if (!resetPassword.Succeeded)
            {
                var errors = resetPassword.Errors.ToList();
                return BadRequest(new { Error = errors });
            }
            return Ok();


        }
        private async Task<bool> UserExists(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
