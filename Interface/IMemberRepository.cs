using AlumniWebsite.API.Configurations.Filter;
using AlumniWebsite.API.Model;
using AlumniWebsite.API.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Interface
{
    public interface IMemberRepository
    {
        Task<PagedList<Member>> GetMembers(MemberParams memberParams);
        Task<Member> GetMember(string id);
        void Update(Member member);
        Task<Member> Login(string email, string password);


    }
}
