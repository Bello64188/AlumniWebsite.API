using AlumniWebsite.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Configurations.Entity
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(k => new { k.LikeeId, k.LikerId });
            builder.HasOne(m => m.Likee)
                .WithMany(m => m.Likers)
                .HasForeignKey(k => k.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.Liker)
                .WithMany(m => m.Likees)
                .HasForeignKey(k => k.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
