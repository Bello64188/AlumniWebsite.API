using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        public IMemberRepository MemberRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
