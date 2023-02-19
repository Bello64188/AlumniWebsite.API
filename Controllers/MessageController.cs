using AlumniWebsite.API.Configurations.Filter;
using AlumniWebsite.API.Interface;
using AlumniWebsite.API.Model;
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
    [ServiceFilter(typeof(LogMemberActivity))]
    [Route("api/members/{memberId}/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unit;

        public MessageController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unit = unitOfWork;
        }
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(string memberId, int id)
        {

            if (memberId != User.Claims.FirstOrDefault(m => m.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value)
                return Unauthorized();
            var message = await _unit.MessageRepository.GetMessage(id);
            if (message == null) return NotFound();
            return Ok(message);
        }
        [HttpGet]
        public async Task<IActionResult> GetMessageForMember([FromQuery] MessageParams messageParams, string memberId)
        {
            if (memberId != User.Claims.FirstOrDefault(m => m.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value)
                return Unauthorized();
            messageParams.MemberId = memberId;
            var messageParamFromRepo = await _unit.MessageRepository.GetMessageForUser(messageParams);
            var message = _mapper.Map<IEnumerable<MessageToReturn>>(messageParamFromRepo);
            Response.AddPagination(messageParamFromRepo.CurrentPage,
                messageParamFromRepo.PageSize,
                messageParamFromRepo.TotalPage,
                messageParamFromRepo.TotalCount);
            return Ok(message);

        }
        [HttpGet("thread/{id}")]
        public async Task<IActionResult> GetMessageThread(string memberId, string id)
        {
            if (memberId != User.Claims.FirstOrDefault(m => m.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value)
                return Unauthorized();
            var messageParamFromRepo = await _unit.MessageRepository.GetMessageThread(memberId, id);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturn>>(messageParamFromRepo);
            return Ok(messageThread);

        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] MessageForCreateDto createDto, string memberId)
        {
            var sender = await _unit.MemberRepository.GetMember(memberId);
            if (sender.Id != User.Claims.FirstOrDefault(m => m.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value)
                return Unauthorized();
            createDto.SenderId = memberId;
            var recipient = await _unit.MemberRepository.GetMember(createDto.RecipientId);
            if (recipient == null) return BadRequest("Cloud not find member");
            var message = _mapper.Map<Message>(createDto);
            _unit.MessageRepository.AddMessage(message);
            if (await _unit.Complete())
            {
                var messageToreturn = _mapper.Map<MessageToReturn>(message);
                return CreatedAtRoute("GetMessage", new { memberId = memberId, id = message.Id }, messageToreturn);
            }
            throw new Exception("Attempt to save Meassage Failed");
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, string memberId)
        {
            if (memberId != User.Claims.FirstOrDefault(m => m.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value)
                return Unauthorized();
            // get message from repository
            var messageFromRepo = await _unit.MessageRepository.GetMessage(id);
            if (messageFromRepo.SenderId == memberId)
                messageFromRepo.SenderDeleted = true;
            if (messageFromRepo.RecipientId == memberId)
                messageFromRepo.RecipientDeleted = true;
            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                _unit.MessageRepository.DeleteMessage(messageFromRepo);
            if (await _unit.Complete())
                return NoContent();
            throw new Exception("Error deleting the message");

        }
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(string memberId, int id)
        {
            if (memberId != User.Claims.FirstOrDefault(m => m.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value)
                return Unauthorized();
            var message = await _unit.MessageRepository.GetMessage(id);
            if (message.RecipientId != memberId)
                return BadRequest("Failed to mark message as read");
            message.IsRead = true;
            message.DateRead = DateTime.Now;
            await _unit.Complete();
            return NoContent();
        }
    }
}
