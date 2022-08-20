using AlumniWebsite.API.Data;
using AlumniWebsite.API.Interface;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.ImplementInterface
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UnitOfWork(AppDbContext context, IMapper mapper, ILoggerFactory logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger.CreateLogger("Logs");
        }

        public IMemberRepository MemberRepository => new MemberRepository(_context, _mapper);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
