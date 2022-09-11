using AlumniWebsite.API.Data;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ImplementInterface
{
    public class LikeRepository : ILikeRepository
    {
        private readonly AppDbContext _context;

        public LikeRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Like like)
        {
            _context.Add(like);
        }

        public async Task<Like> GetLike(string memberId, string recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == memberId && u.LikeeId == recipientId);
        }
    }
}
