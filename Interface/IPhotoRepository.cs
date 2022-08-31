using AlumniWebsite.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Interface
{
    public interface IPhotoRepository
    {
        Task<Photo> GetPhotoById(int id);
        Task<Photo> GetMainPhotoForMember(string memberId);
        void RemovePhoto(Photo photo);
    }
}
