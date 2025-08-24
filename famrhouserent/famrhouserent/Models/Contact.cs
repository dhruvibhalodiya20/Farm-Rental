using System.ComponentModel.DataAnnotations.Schema;

namespace famrhouserent.Models
{
    //public class Contact
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string Phone { get; set; } = string.Empty;
    //    public string Subject { get; set; } = string.Empty;
    //    public string Message { get; set; } = string.Empty;
    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //    public bool IsRead { get; set; } = false;

    //    public string? AdminReply { get; set; }


    //}
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;

        // Add this:
        public string? AdminReply { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public int? UserId { get; set; }
        public User? User { get; set; }

    }

}
