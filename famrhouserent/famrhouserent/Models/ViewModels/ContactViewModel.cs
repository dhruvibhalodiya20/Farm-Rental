using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace famrhouserent.Models.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a subject")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your message")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters")]
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string? AdminReply { get; set; }
        [ForeignKey("UserId")]
        public int? UserId { get; set; }
        public User? User { get; set; }



    }
}
