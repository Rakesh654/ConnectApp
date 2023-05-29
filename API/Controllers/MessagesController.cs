using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
             var userName = User.GetUserName();
             if(userName == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot send the message to yourself");

             var sender = await _userRepository.GetUserByUserNameAsync(userName);
             var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername); 

             if(recipient == null) return NotFound();

             var messgae = new Message
             {
                 Sender = sender,
                 Recipient = recipient,
                 SenderUsername = sender.UserName,
                 RecipientUsername = recipient.UserName,
                 Content = createMessageDto.Content   
             };

             _messageRepository.AddMessage(messgae);

             if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(messgae));

             return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PageList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var message = await _messageRepository.GetMessageForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(message.CurrentPage, message.PageSize, message.TotalCount,message.TotalPages));
            return message;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUserName = User.GetUserName();
            return Ok(await _messageRepository.GetMessageThread(currentUserName, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();
            var message = await _messageRepository.GetMessage(id);
            if(message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();

            if(message.SenderUsername == username) message.SenderDeleted = true;
            if(message.RecipientUsername == username) message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            if(await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem in deleting message");
        }
    }
}