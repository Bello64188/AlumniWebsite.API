using AlumniWebsite.API.Data;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using AlumniWebsite.API.ModelDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ImplementInterface
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly AppDbContext _context;

        public PhotoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Photo> GetMainPhotoForMember(string memberId)
        {
            var member = await _context.Photos.
                Where(m => m.MemberId == memberId).FirstOrDefaultAsync(i => i.IsMain);
            return member;
        }

        public async Task<Photo> GetPhotoById(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(u => u.Id == id);
            return photo;
        }

        public async Task<IEnumerable<ApprovePhotoDto>> GetUnApprovePhoto()
        {
            var unApprovedPhoto = await _context.Photos.IgnoreQueryFilters().Where(p => p.PublicId != null).Select(p => new ApprovePhotoDto
            {
                Id = p.Id,
                IsApproved = p.IsApproved,
                Url = p.Url,
                UserName = p.Member.UserName

            }).ToListAsync();
            return (unApprovedPhoto);
        }

        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }
    }
}
