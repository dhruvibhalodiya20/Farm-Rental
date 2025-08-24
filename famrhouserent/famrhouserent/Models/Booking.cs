using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using famrhouserent.Models;

namespace famrhouserent.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int FarmHouseId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Guests must be greater than 0.")]
        public int Guests { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TotalPrice { get; set; }

        [MaxLength(50)]
        public string BookingStatus { get; set; } = "Pending";

        public DateTime BookingDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User UserAccount { get; set; }

        [ForeignKey("FarmHouseId")]
        public virtual FarmHouse FarmHouse { get; set; }
    }
}
