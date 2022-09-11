using AlumniWebsite.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Interface
{
    public interface ILikeRepository
    {
        Task<Like> GetLike(string memberId, string recipientId);
        void Add(Like like);

    }
}
