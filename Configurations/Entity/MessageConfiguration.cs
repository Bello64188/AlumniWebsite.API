using AlumniWebsite.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Configurations.Entity
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasOne(u => u.Sender)
                  .WithMany(u => u.MessageSent)
                  .HasForeignKey(u => u.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Recipient)
                .WithMany(r => r.MessageReceived)
                .HasForeignKey(r => r.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
