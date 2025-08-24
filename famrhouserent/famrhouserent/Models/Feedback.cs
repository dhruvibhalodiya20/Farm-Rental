using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace famrhouserent.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        //[Required]
        public int UserId { get; set; }

        [Range(1, 5)]
        [Required]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public User UserAccounts { get; set; } = null!;
    }
}
