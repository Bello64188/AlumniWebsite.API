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
            members = members.Where(g => g.Gender == memberParams.Gender);
            // filter with age
            if (memberParams.MinAge != 18 || memberParams.MaxAge != 99)
            {
                DateTime maxDob = DateTime.Today.AddYears(-memberParams.MinAge);
                DateTime minDob = DateTime.Today.AddYears(-memberParams.MaxAge - 1);
                members = members.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            // sorting by created and lastActive
            if (!string.IsNullOrEmpty(memberParams.OrderBy))
            {
                switch (memberParams.OrderBy.ToLower())
                {
                    case "created":
                        members.OrderByDescending(c => c.Created);
                        break;
                    default:
                        members.OrderByDescending(m => m.LastActive);
                        break;
                }
            }
            // return member
            return await PagedList<Member>.CreatAsync(members, memberParams.PageNumber, memberParams.PageSize);
        }

        public async Task<Member> Login(string email, string password)
        {
            var member = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(e => e.Email == email);
            if (member == null) return null;
            return member;
        }
    }
}
