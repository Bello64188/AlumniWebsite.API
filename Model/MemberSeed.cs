using AlumniWebsite.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Model
{
    public class MemberSeed
    {


        public static async Task SeedMembersAsync(UserManager<Member> userManager,
            RoleManager<MemberRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            var memberDate = await System.IO.File.ReadAllTextAsync("Data/MemberSeedData.json");
            var members = JsonConvert.DeserializeObject<List<Member>>(memberDate);
            if (members == null) return;
            var roles = new List<MemberRole>
            {
                new MemberRole{ Name = "Member"},
                new MemberRole{ Name = "Admin"},
                new MemberRole{ Name = "Moderator"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var member in members)
            {
                member.Photos.First().IsApproved = true;
                member.UserName = member.UserName.ToLower();
                await userManager.CreateAsync(member, "Pa$$word1");
                await userManager.AddToRoleAsync(member, "Member");
            }
            var admin = new Member
            {
                Email = "admin@gmail.com",
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "Pa$$word1");
            await userManager.AddToRoleAsync(admin, new[] { "Admin", "Moderator" }.ToString());
        }
    }
}
