using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatController(ApplicationDbContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // 🔹 API 1: SEND MESSAGE
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var chatMessage = new ChatMessage
        {
            SenderId = dto.SenderId,
            SenderRole = dto.SenderRole,
            SenderName = dto.SenderName,
            Message = dto.Message,
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        var response = new ChatMessageDto
        {
            Id = chatMessage.Id,
            SenderId = chatMessage.SenderId,
            SenderRole = chatMessage.SenderRole,
            SenderName = chatMessage.SenderName,
            Message = chatMessage.Message,
            SentAt = chatMessage.SentAt
        };

        // 🔥 Broadcast validated DTO
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", response);

        return Ok(response);
    }

    // 🔹 API 2: GET MESSAGE HISTORY
    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages()
    {
        var messages = await _context.ChatMessages
            .OrderBy(m => m.SentAt)
            .Take(100)
            .Select(m => new ChatMessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderRole = m.SenderRole,
                SenderName = m.SenderName,
                Message = m.Message,
                SentAt = m.SentAt
            })
            .ToListAsync();

        return Ok(messages);
    }
}
