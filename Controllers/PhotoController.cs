using AlumniWebsite.API.Configurations.CloudConfiguration;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using AlumniWebsite.API.ModelDto;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Controllers
{
    [Route("api/members/{memberId}/[controller]")]
    [ApiController]
    [Authorize]
    public class PhotoController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<Cloud> _cloudSetting;
        private readonly IMapper _mapper;
        private Cloudinary _cloudinary;

        public PhotoController(IUnitOfWork unitOfWork, IMapper mapper,
            IOptions<Cloud> cloudSetting)
        {
            _unitOfWork = unitOfWork;
            _cloudSetting = cloudSetting;
            _mapper = mapper;
            Account acc = new Account(
                _cloudSetting.Value.CloudName,
                _cloudSetting.Value.ApiKey,
                _cloudSetting.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name = "GetMemberPhoto")]
        public async Task<IActionResult> GetMemberPhoto(int id)
        {
            var photoFromRepo = await _unitOfWork.PhotoRepository.GetPhotoById(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddMemberPhoto(string memberId, [FromForm] PhotoForCreationDto photoForCreation)
        {
            var member = await _unitOfWork.MemberRepository.GetMember(memberId);
            if (member == null)
                return BadRequest("Member Not Found!!");
            var file = photoForCreation.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Folder = "ModebeAlumni/image",
                        Transformation = new Transformation().Width(500)
                        .Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreation.Url = uploadResult.Url.ToString();
            photoForCreation.PublicId = uploadResult.PublicId;
            var photo = _mapper.Map<Photo>(photoForCreation);
            photo.Member = member;
            if (!member.Photos.Any(i => i.IsMain))
                photo.IsMain = true;
            member.Photos.Add(photo);
            if (!await _unitOfWork.Complete())
                return BadRequest("Cloud not add the photo");
            var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
            return CreatedAtRoute(nameof(GetMemberPhoto),
                new { memberId, id = photo.Id }, photoToReturn);
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(string memberId, int id)
        {
            var member = await _unitOfWork.MemberRepository.GetMember(memberId);
            if (!member.Photos.Any(p => p.Id == id))
                return Unauthorized();
            var photoFromRepo = await _unitOfWork.PhotoRepository.GetPhotoById(id);
            if (photoFromRepo.IsMain)
                return BadRequest("This is Already the main photo");
            var currentPhoto = await _unitOfWork.PhotoRepository.GetMainPhotoForMember(memberId);
            currentPhoto.IsMain = false;
            photoFromRepo.IsMain = true;
            if (await _unitOfWork.Complete())
                return NoContent();
            return BadRequest("Failed to set photo has main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMemberPhoto(string memberId, int id)
        {
            var member = await _unitOfWork.MemberRepository.GetMember(memberId);
            if (!member.Photos.Any(p => p.Id == id))
                return Unauthorized();
            var photofromRepo = await _unitOfWork.PhotoRepository.GetPhotoById(id);
            if (photofromRepo.IsMain)
                return BadRequest("You cannot delete the main Photo");
            if (photofromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photofromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                    _unitOfWork.PhotoRepository.RemovePhoto(photofromRepo);
                if (photofromRepo.PublicId == null)
                    _unitOfWork.PhotoRepository.RemovePhoto(photofromRepo);
                if (await _unitOfWork.Complete())
                    return Ok();

            }
            return BadRequest("Unable to delete the Photo..");
        }
    }
}
