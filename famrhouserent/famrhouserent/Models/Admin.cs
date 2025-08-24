using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace famrhouserent.Models
{
    [Table("tbladmin")]
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
