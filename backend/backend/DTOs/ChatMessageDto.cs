using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class ChatMessageDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public string SenderRole { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime SentAt { get; set; }
    }
}