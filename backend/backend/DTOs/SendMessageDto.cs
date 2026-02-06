using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "SenderId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "SenderId must be valid")]
        public int SenderId { get; set; }

        [Required(ErrorMessage = "SenderRole is required")]
        [RegularExpression(
            "Doctor|Patient|Admin",
            ErrorMessage = "SenderRole must be Doctor, Patient, or Admin"
        )]
        public string SenderRole { get; set; }

        [Required(ErrorMessage = "SenderName is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "SenderName must be between 2 and 100 characters")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, MinimumLength = 1,
            ErrorMessage = "Message must be between 1 and 1000 characters")]
        public string Message { get; set; }
    }
}