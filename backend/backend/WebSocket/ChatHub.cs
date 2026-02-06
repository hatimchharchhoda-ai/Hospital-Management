using backend.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace backend.WebSocket
{
    public class ChatHub : Hub
    {
        public async Task BroadcastMessage(ChatMessageDto message)
        {
            if (message == null)
                throw new HubException("Invalid message");

            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}