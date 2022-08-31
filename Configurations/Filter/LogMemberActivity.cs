using AlumniWebsite.API.ImplementInterface;
using AlumniWebsite.API.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AlumniWebsite.API.Configurations.Filter
{
    public class LogMemberActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logContext = await next();
            var memberId = logContext.HttpContext.User.Claims.FirstOrDefault(
                t => t.Type.Equals("id", StringComparison.OrdinalIgnoreCase))?.Value;
            var repo = logContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var resultMember = await repo.MemberRepository.GetMember((memberId));
            resultMember.LastActive = DateTime.Now;
            await repo.Complete();

        }
    }
}
