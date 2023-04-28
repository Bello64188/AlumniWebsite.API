using AlumniWebsite.API.Configurations.Filter;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Controllers
{
    [ServiceFilter(typeof(LogMemberActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<Member> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPhotoService photoService;
        private readonly RoleManager<MemberRole> roleManager;

        public AdminController(UserManager<Member> userManager, IUnitOfWork unitOfWork,
            IPhotoService photoService, RoleManager<MemberRole> roleManager)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.photoService = photoService;
            this.roleManager = roleManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "RequireAdminRole", Roles = "Admin")]
        [HttpGet("users-with-roles")]
        public async Task<IActionResult> GetMemberWithRole()
        {
            var members = await userManager.Users
                  .Include(p => p.MemberRoles)
                  .OrderBy(m => m.UserName)
                  .Select(m => new
                  {
                      m.Id,
                      UserName = m.UserName,
                      Roles = m.MemberRoles.Select(r => r.Name)
                  }).ToListAsync();
            return Ok(members);
        }
        [HttpPost("edit-roles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, [FromQuery] string roles)
        {
            var chooseRoles = roles.Split(",").ToArray();
            var member = await userManager.FindByNameAsync(userName);
            if (member == null) return NotFound("Member Not Found");
            var memberRoles = await userManager.GetRolesAsync(member);
            var response = await userManager.AddToRolesAsync(member, chooseRoles.Except(memberRoles));
            if (!response.Succeeded) return BadRequest("Failed to add to roles");
            //foreach (var item in chooseRoles)
            //{
            //    if (item.ToLower() == "admin" || item.ToLower() == "moderator")
            //    {
            //        var role = new MemberRole
            //        {
            //            Name = item
            //        };
            //        await roleManager.CreateAsync(role);
            //    }


            //}

            response = await userManager.RemoveFromRolesAsync(member, memberRoles.Except(chooseRoles));
            if (!response.Succeeded) return BadRequest("Failed to remove from roles");
            return Ok(await userManager.GetRolesAsync(member));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ModeratePhotoRole")]
        [HttpGet("photo-to-moderate")]
        public async Task<IActionResult> GetUnapprovedPhoto()
        {

            var photo = await unitOfWork.PhotoRepository.GetUnApprovePhoto();
            return Ok(photo);
        }

        [HttpPost("approve-photo/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if (photo == null) return NotFound("Could not find photo");
            photo.IsApproved = true;
            var user = await unitOfWork.MemberRepository.GetUserByPhotoId(photoId);
            if (!user.Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }
            await unitOfWork.Complete();
            return Ok();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<IActionResult> RejectPhoto(int photoId)
        {

            var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhoto(photo.PublicId);
                if (result.Result == "ok")
                {
                    unitOfWork.PhotoRepository.RemovePhoto(photo);
                }

            }
            else
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);

            }
            await unitOfWork.Complete();
            return Ok();
        }

    }
}
