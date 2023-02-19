using AlumniWebsite.API.Configurations.CloudConfiguration;
using AlumniWebsite.API.Interface;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IOptions<Cloud> cloudSetting;
        private Cloudinary _cloudinary;

        public PhotoService(IOptions<Cloud> cloudSetting)
        {
            this.cloudSetting = cloudSetting;
            Account acc = new Account(
               this.cloudSetting.Value.CloudName,
               this.cloudSetting.Value.ApiKey,
               this.cloudSetting.Value.ApiSecret
               );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhoto(IFormFile file)
        {
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
            return await Task.FromResult(uploadResult);
        }

        public async Task<DeletionResult> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deleteParams);
            return await Task.FromResult(result);
        }
    }
}
