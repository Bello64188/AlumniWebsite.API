using AlumniWebsite.API.Configurations.Entity;
using AlumniWebsite.API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Data
{
    public class AppDbContext : IdentityDbContext<Member, MemberRole, string,
                 IdentityUserClaim<string>, MemberUserRole, IdentityUserLogin<string>,
                 IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new LikeConfiguration());
        }
    }
}
