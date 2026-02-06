using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SenderRole { get; set; }   // Doctor, Patient, Admin

        [Required]
        [MaxLength(100)]
        public string SenderName { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }

        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
