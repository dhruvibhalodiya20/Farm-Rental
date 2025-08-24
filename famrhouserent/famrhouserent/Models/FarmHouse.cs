using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace famrhouserent.Models
{
    [Table("FarmHouse")]
    public class FarmHouse
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string Location { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Bedrooms { get; set; }
        public string Amenities { get; set; }
        public int MaxGuests { get; set; }
        public decimal PricePerNight { get; set; }

        public int OwnerId { get; set; }
        public Owner Owner { get; set; }

        // Navigation property
        //public List<FarmHouseImage> Images { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<FarmHouseImage> FarmHouseImages { get; set; }

        //  public List<string>? ExistingImageUrls { get; set; }




    }
}