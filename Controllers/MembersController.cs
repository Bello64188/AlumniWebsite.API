using AlumniWebsite.API.Configurations.Filter;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.ModelDto;
using AlumniWebsite.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMembers([FromQuery] MemberParams memberParams)
        {
            var currentId = User.Claims.FirstOrDefault(getType => getType.Type.Equals("id", StringComparison.OrdinalIgnoreCase))?.Value;
            if (string.IsNullOrEmpty(currentId)) return NotFound();
            var memberFromUnit = await _unitOfWork.MemberRepository.GetMember(currentId);
            if (memberFromUnit == null) return Unauthorized();
            memberParams.MemberId = currentId;
            if (string.IsNullOrEmpty(memberParams.Gender))
            {
                memberParams.Gender = memberFromUnit.Gender.ToLower() == "male" ? "female" : "male";
            }
            var member = await _unitOfWork.MemberRepository.GetMembers(memberParams);
            var memberMap = _mapper.Map<IList<MemberListDto>>(member);
            Response.AddPagination(member.CurrentPage, member.PageSize, member.TotalPage, member.TotalCount);
            return Ok(memberMap);
        }
        [HttpGet("{id}", Name = "GetMember")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMember(string id)
        {
            var member = await _unitOfWork.MemberRepository.GetMember(id);
            var memberMap = _mapper.Map<MemberDetailsDto>(member);
            return Ok(memberMap);
        }
    }
}
