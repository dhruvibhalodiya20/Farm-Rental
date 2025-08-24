using System.ComponentModel.DataAnnotations;

namespace famrhouserent.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
  
        public string? Name { get; set; }

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetExpiry { get; set; }

        public bool IsVerified { get; set; }

        public bool IsAdmin { get; set; } = false;

        public string? Otp { get; set; }

        public DateTime? OtpExpiry { get; set; }
        public ICollection<Booking>? Bookings { get; set; }

    }
}
