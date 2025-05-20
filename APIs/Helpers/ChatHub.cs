using APIs.DTO;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace APIs.Helpers
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string receiverId , string message)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var messageDto = new MessageDto
            {
                SenderName = senderId,
                ReceiverName = receiverId,
                Content = message,
                SentAt = DateTime.UtcNow
            };
            //Here we Using NameUserIdProvider Class
            await Clients.User(receiverId).SendAsync("ReceiveMessage", messageDto);
        }

    }
}
