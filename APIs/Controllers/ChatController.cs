using APIs.DTO;
using APIs.Errors;
using APIs.Helpers;
using AutoMapper;
using Core.Entites.Identity;
using Core.Entites.SignalR;
using Core.IRepository;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Repository;
using System.Reflection;
using System.Security.Claims;

namespace APIs.Controllers
{
    public class ChatController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<ChatHub> hubContext;

        public ChatController(IUnitOfWork _unitOfWork , IMapper _mapper,
            UserManager<ApplicationUser> _userManager , IHubContext<ChatHub> _hubContext)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userManager = _userManager;
            hubContext = _hubContext;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> SendMessage (SendMessageDto messageDto)
        {
            var recever = await userManager.FindByEmailAsync(messageDto.SendToEmail);
            if (recever == null)     
                return NotFound(new ApiResponce(404, $"Are You Sure That {messageDto.SendToEmail} is Correct"));
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new Message()
            {
                SenderId = senderId,
                ReceiverId = recever.Id,
                Content = messageDto.Content,
                SentAt = DateTime.UtcNow
            };
            await unitOfWork.Entity<Message>().AddAsync(message);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400, "The Message Failed"));
            await hubContext.Clients.User(recever.Id).SendAsync("ReceiveMessage", message.Content);
            return $"The Message '{messageDto.Content}' Successfully Sent";
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<MessageDto>) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<MessageDto>>> GetMessages(string Email)
        {
            var Resever = await userManager.FindByEmailAsync(Email);
            if (Resever == null)
                return NotFound(new ApiResponce(404, $"Are You Sure That {Email} is Correct"));
            var SenderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var Sedner = await userManager.FindByIdAsync(SenderId);
            var messageSpec = new MessageSpec(SenderId, Resever.Id);
            var messages = await unitOfWork.Entity<Message>().GetAllIncludedAsync(messageSpec,true);
            if(!messages.Any())
                return NotFound(new ApiResponce(404 , $"There Is No Messages Between You And {Resever.FullName}"));
            var messagedto = messages.Select(I => new MessageDto()
            {
                ReceiverName = Resever.FullName,
                SenderName = Sedner.FullName,
                Content = I.Content,
                SentAt = I.SentAt
            }).ToList();
            return Ok(messagedto);
        }

    }
}
