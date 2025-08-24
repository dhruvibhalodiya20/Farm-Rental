using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace famrhouserent.Models
{
    public class FarmHouseImage
    {
        [Key]
        public int ImageId { get; set; }

        [ForeignKey("FarmHouse")]
        public int FarmHouseId { get; set; }

        [Required]
        [MaxLength(300)]
        public string ImageUrl { get; set; }

        public FarmHouse FarmHouse { get; set; }
    }
}
