using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace famrhouserent.Models
{
    [Table("tblOwner")]
    public class Owner
    {
        [Key]
        public int OwnerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

       
        public ICollection<FarmHouse>? FarmHouses { get; set; }

    }
}
