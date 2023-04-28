using AlumniWebsite.API.Configurations.Filter;
using AlumniWebsite.API.Data;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
using AlumniWebsite.API.ModelDto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ImplementInterface
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MemberRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Member> GetMember(string id)
        {
            var member = await _context.Users.Include(includePhoto => includePhoto.Photos)
                  .FirstOrDefaultAsync(i => i.Id == id);
            return member;
        }

        public async Task<PagedList<Member>> GetMembers(MemberParams memberParams)
        {
            var members = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            //filtering region
            /// <summary>
            /// 1. do not display logged in user on the list
            /// 2. select  member gender where is equal to params gender 
            /// </summary>
            members = members.Where(m => m.Id != memberParams.MemberId);
            members = members.Where(g => g.GraduationYear == memberParams.GraduationYear);
            if (memberParams.Gender != null)
            {
                members = members.Where(g => g.Gender == memberParams.Gender && g.GraduationYear == memberParams.GraduationYear);
            }

            // member i liked
            if (memberParams.Likers)
            {
                var memberLikers = await GetMemberLike(memberParams.MemberId, memberParams.Likers);
                members = members.Where(m => memberLikers.Contains(m.Id));
            }
            if (memberParams.Likees)
            {
                var memberLikees = await GetMemberLike(memberParams.MemberId, memberParams.Likers);
                members = members.Where(m => memberLikees.Contains(m.Id));
            }


            // if graduationYear is not equal to null
            if (memberParams.GraduationYear != null)
            {
                members = members.Where(y => y.GraduationYear == memberParams.GraduationYear);
            }
            // sorting by created and lastActive

            if (!string.IsNullOrEmpty(memberParams.OrderBy))
            {
                switch (memberParams.OrderBy.ToLower())
                {
                    case "created":
                        members = members.OrderByDescending(c => c.Created);
                        break;
                    default:
                        members = members.OrderByDescending(m => m.LastActive);
                        break;
                }
            }
            // return member
            return await PagedList<Member>.CreatAsync(members, memberParams.PageNumber, memberParams.PageSize);
        }

        private async Task<IEnumerable<string>> GetMemberLike(string memberId, bool likers)
        {
            var member = await _context.Users
                 .Include(l => l.Likers)
                 .Include(l => l.Likees)
                 .FirstOrDefaultAsync(i => i.Id == memberId);
            if (likers)
                return member.Likers.Where(i => i.LikerId == memberId).Select(i => i.LikeeId);
            else
                return member.Likees.Where(i => i.LikeeId == memberId).Select(i => i.LikerId);
        }

        public async Task<Member> Login(string email, string password)
        {
            var member = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(e => e.Email == email);
            if (member == null) return null;
            return member;
        }

        public void Update(Member member)
        {
            _context.Update(member);
        }

        public async Task<Member> GetUserByPhotoId(int id)
        {
            var user = await _context.Users.Include(p => p.Photos)
                .IgnoreQueryFilters().Where(l => l.Photos.Any(p => p.Id == id))
                .FirstOrDefaultAsync();
            return await Task.FromResult(user);
        }
    }
}
